using System;
using System.Collections.Generic;
using CameraUtils;
using CharacterAttributes.PlayerUI;
using GameEvents;
using GameProps;
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
    [RequireComponent(typeof(AnimationCurve))]
    [RequireComponent(typeof(CinemachineImpulseSource))]
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
        public float speed           = 10f;
        public float speedChangeRate = 20f;
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
        [Header("Pushed")] 
        public float pushedSpeed = 80f;
        public float pushedTime = 0.4f;

        [Header("Default tag ball count")] public int tagBalls = 5;

        [Header("Mass")] public float mass = 60f;
        
        [Header("Player Dash(Skill)")] 
        public float dashDuration = 3f;
        public float          dashSpeed       = 60f;
        public float          bounceBackForce = 2f;
        public AnimationCurve dashCurve;
        
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
            if (!context.performed || !IsOwner) 
                return;

            if (_fsm.IsStateActive(FSMType.ATTACK)) 
                return;

            _fsm.SetState(FSMType.ATTACK);
        }

        public void OnSkill(InputAction.CallbackContext context)
        {
            if (!context.performed || !IsOwner) 
                return;
            
            if(_fsm.IsStateActive(FSMType.SKILL))
                return;
            
            _fsm.FinishAllStates();
            _fsm.SetState(FSMType.SKILL);
        }
        
        #endregion

        #region Server_Only

        // 클래스 안에 추가
        private static Dictionary<(ulong, ulong), Dictionary<ulong, float>> _crashBuffer = new();
        public         float                                                crashFactor;
        private        HashSet<ulong>                                       _alreadyCrashedWith = new HashSet<ulong>();
        #endregion

        #region PlayerUI

        public PlayerUIController playerUIController;

        #endregion

        #region Cinemachine

        public CinemachineImpulseSource impulseSource;        

        #endregion
        [Space(5)]
        [Header("Animations")]
        #region Animations
        public Animator animator;        
        #endregion
        
        public override void OnNetworkSpawn()
        {
            characterController    = this.GetComponent<CharacterController>();
            playerInput            = this.GetComponent<PlayerInput>();
            characterStatusNetwork = this.GetComponent<CharacterStatusNetwork>();
            playerUIController     = this.GetComponent<PlayerUIController>();
            
            if (!IsOwner)
            {
                playerInput.enabled = false;
                this.transform.tag  = "Hostile";
                return;
            }

            //if (IsServer && IsOwner)
            {
                //TODO: 장비에서 스텟 받아오기
                characterStatusNetwork.Initialize(100f,pushedSpeed,10f, pushedTime, speed, tagBalls, mass);
                crashFactor = speed;
            }

            impulseSource = this.GetComponent<CinemachineImpulseSource>();
            
            playerUIController.Initialize(this);
            this.transform.tag = "Player";
            
            mainCamera = Camera.main;

            var vCams = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
            foreach (var cam in vCams)
            {
                cam.Follow = cinemachineCameraTarget;
            }
            //GameObject.FindAnyObjectByType<CinemachineCamera>().Follow = cinemachineCameraTarget;
            
            _cinemachineTargetYaw = cinemachineCameraTarget.eulerAngles.y;

            if (!GameObject.FindAnyObjectByType<PlayerEffectCameraController>())
            {
                Debug.Log("Failed to find Effect Camera");
            }
            else
            {
                //Debug.Log("Found Effect Camera... Setting....");
                var effectCam = GameObject.FindAnyObjectByType<PlayerEffectCameraController>();
                effectCam.cinemachineImpulseSource = impulseSource;
            }
            
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
                
                ulong otherId = otherController.NetworkObjectId;

                if (_alreadyCrashedWith.Contains(otherId)) return;

                _alreadyCrashedWith.Add(otherId);
                
                Debug.Log($"My Speed is {crashFactor}");
                ReportCrashDataServerRpc(NetworkObjectId, otherController.NetworkObjectId, crashFactor);
            }
            else if (other.CompareTag($"NetworkProp"))
            {
                var food = other.GetComponent<FoodBase>();
                if (food != null)
                {
                    // 요청은 클라이언트가 하지만, 체력 감소는 서버에서만 실행됨
                    food.TakeDamage(100f);
                }
            }
        }

        [ServerRpc]
        private void ReportCrashDataServerRpc(ulong senderId, ulong otherId, float crashFactor_)
        {
            var key = senderId < otherId ? (senderId, otherId) : (otherId, senderId);

            if (!_crashBuffer.ContainsKey(key))
            {
                _crashBuffer[key] = new Dictionary<ulong, float>();
            }

            _crashBuffer[key][senderId] = crashFactor_;
            
            if (_crashBuffer[key].Count == 2)
            {
                float senderCrash = _crashBuffer[key][key.Item1];
                float targetCrash = _crashBuffer[key][key.Item2];

                var senderObj = NetworkManager.SpawnManager.SpawnedObjects[key.Item1];
                var targetObj = NetworkManager.SpawnManager.SpawnedObjects[key.Item2];

                var senderController = senderObj.GetComponent<CharacterControllerNetwork>();
                var targetController = targetObj.GetComponent<CharacterControllerNetwork>();

                Debug.Log($"Collision! {key.Item1} speed: {senderCrash}, {key.Item2} speed: {targetCrash}");

                if (senderController != null)
                    senderController.ReceiveHitClientRpc(targetCrash);

                if (targetController != null)
                    targetController.ReceiveHitClientRpc(senderCrash);

                _crashBuffer.Remove(key); // 처리 후 버퍼 제거
            }
        }

        [ClientRpc]
        private void ReceiveHitClientRpc(float damage,ClientRpcParams rpcParams = default)
        {
            if (!IsOwner)
                return;

            _alreadyCrashedWith.Clear();
            GameEvents.GameEvents.Publish(GameEventType.PLAYER_CRASH);
            Debug.Log($"Received Hit damage : {damage}");
            characterStatusNetwork.TakeDamage(damage);
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
