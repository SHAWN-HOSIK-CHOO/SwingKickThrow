using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace CharacterAttributes
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(CharacterStatusNetwork))]
    public class CharacterControllerNetwork : NetworkBehaviour
    {
        public  CharacterController characterController;
        private CharacterFSMContext _fsm;

        [Header("Hoverboard Behaviour")]
        #region HoverBoardBehaviour
        public float maxRollAngle     = 15f;
        public float rollSensitivity  = 5f;
        public float maxPitchAngle    = 10f;
        public float pitchSensitivity = 5f;
        public float rayOffset        = 0.75f;
        public float rayLength        = 1.2f;
        #endregion
        
        [Space(5)]
        [Header("Camera Settings")]
        #region Camera
        public        Camera    mainCamera;
        public        Transform cinemachineCameraTarget;
        private       float     _cinemachineTargetYaw;
        private       float     _cinemachineTargetPitch;
        private const float     CameraThreshold        = 0.01f;
        public        float     topClamp               = 23f;
        public        float     bottomClamp            = 10f;
        public        float     cameraYawSensitivity   = 0.5f;
        public        float     cameraPitchSensitivity = 0.5f;
        #endregion
        
        [Space(5)]
        [Header("Player Settings")]
        public CharacterStatusNetwork characterStatusNetwork;
        public float speed           = 20f;
        public float speedChangeRate = 10f;
        public float groundOffset    = -0.14f;
        public float gravity         = -15f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float fallTimeout = 0.15f;
        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool grounded = true;
        [Tooltip("Useful for rough ground")]
        public float groundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float groundedRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask groundLayers;
        public float terminalVelocity = 53f;
        [Tooltip("How high is the origin from the ground")]
        public float originOffset = 0.6f;

        [Header("Player Dash(Skill)")] 
        public float dashDuration = 0.3f;
        public float dashSpeed = 35f;
        public float bounceBackForce = 2f;
        
        [Space(5)]
        [Header("PlayerInput")]
        [Header("알 수 없는 오류 때문에 수동으로 콜백 연결해줘야 함")]
        #region PlayerInputs
        public  PlayerInput         playerInput;
        public Vector2 MoveInput { get; set; }
        public Vector2 LookInput { get; set; }
        public bool    IsAttack  { get; set; }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;

            if (_fsm.IsStateActive(FSMType.ATTACK)) 
                return;

            _fsm.SetState(FSMType.ATTACK);
        }

        public void OnSkill(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
            
            if(_fsm.IsStateActive(FSMType.SKILL))
                return;
            
            _fsm.FinishAllStates();
            _fsm.SetState(FSMType.SKILL);
        }
        
        #endregion

        [Space(5)]
        [Header("Animations")]
        #region Animations
        public Animator animator;        
        #endregion
        
        public override void OnNetworkSpawn()
        {
            characterController     = this.GetComponent<CharacterController>();
            playerInput             = this.GetComponent<PlayerInput>();
            characterStatusNetwork = this.GetComponent<CharacterStatusNetwork>();
            
            //TODO: 장비에서 스텟 받아오기
            characterStatusNetwork.Initialize(100f,10f,10f);
            
            if (!IsOwner)
            {
                playerInput.enabled = false;
                this.transform.tag  = "Hostile";
                return;
            }

            this.transform.tag = "Player";
            
            mainCamera = Camera.main;

            GameObject.FindAnyObjectByType<CinemachineCamera>().Follow = cinemachineCameraTarget;

            _cinemachineTargetYaw = cinemachineCameraTarget.eulerAngles.y;
            
            
            _fsm = new CharacterFSMContext(this);
            _fsm.Initialize();

            _fsm.SetState(FSMType.MOVEMENT); // 기본 상태
        }

        private void Update() {
            if (!IsOwner) 
                return;
            
            _fsm.OnUpdate();
        }

        private void LateUpdate()
        {
            if(!IsOwner)
                return;
            
            CameraRotation();
        }

        private void CameraRotation()
        {
            if (LookInput.sqrMagnitude >= CameraThreshold)
            {
                _cinemachineTargetYaw   += LookInput.x * cameraYawSensitivity;
                _cinemachineTargetPitch -= LookInput.y * cameraPitchSensitivity;
            }

            _cinemachineTargetYaw   = ClampAngle(_cinemachineTargetYaw,   float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp,    topClamp);
            
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,_cinemachineTargetYaw,0f);
        }
        
        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (hit.collider.CompareTag(tag: "Hostile")) {
                //fsm.OnHit(hit.gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!IsOwner)
                return;

            if (other.CompareTag("Hostile"))
            {
                var otherController = other.GetComponent<CharacterControllerNetwork>();
                if (otherController == null)
                    return;

                // 본인과 상대방의 NetworkObjectId 전송 (ClientId 사용도 가능)
                ReportHitServerRpc(NetworkObjectId, otherController.NetworkObjectId);
            }
        }

        [ServerRpc]
        private void ReportHitServerRpc(ulong senderId, ulong targetId)
        {
            if (senderId >= targetId)
                return; // 항상 낮은 ID를 가진 쪽만 처리 주체

            var senderObj = NetworkManager.SpawnManager.SpawnedObjects[senderId];
            var targetObj = NetworkManager.SpawnManager.SpawnedObjects[targetId];

            var senderController = senderObj.GetComponent<CharacterControllerNetwork>();
            var targetController = targetObj.GetComponent<CharacterControllerNetwork>();

            if (senderController != null)
            {
                senderController.ReceiveHitClientRpc();
            }

            if (targetController != null)
            {
                targetController.ReceiveHitClientRpc();
            }
        }


        [ClientRpc]
        private void ReceiveHitClientRpc(ClientRpcParams rpcParams = default)
        {
            if (!IsOwner)
                return;

            Debug.Log("Received Hit -> FSM Set HIT");
            _fsm.FinishAllStates();
            _fsm.SetState(FSMType.HIT);
        }

        
        #region Static 

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle  -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        #endregion
    }
}
