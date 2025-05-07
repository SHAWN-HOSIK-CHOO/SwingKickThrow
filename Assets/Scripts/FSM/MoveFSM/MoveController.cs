using FSM;
using TickSystem;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class MoveController : MonoBehaviour, IOwner<MoveController>
    {
        public TickController _tickContoller { get; private set; }
        public CharacterController _characterController { get; private set; }
        public InputActionMap inputActions { get; private set; } 

        IFSM<MoveState,MoveController> _fsm;
        public MoveController Get => this;

        [Space(5)]
        [Header("Camera Settings")]
        #region Camera
        public CinemachineCamera _playerCamera;
        private const float CameraThreshold = 0.01f;
        public float topClamp = 23f;
        public float bottomClamp = 10f;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        public float cameraYawSensitivity = 0.5f;
        public float cameraPitchSensitivity = 0.5f;
        #endregion

        [Header("Hoverboard Behaviour")]
        #region HoverBoardBehaviour
        public float _maxRollAngle = 15f;
        public float _rollSensitivity = 5f;
        public float _maxPitchAngle = 10f;
        public float _pitchSensitivity = 5f;
        public float _rayOffset = 0.75f;
        public float _rayLength = 1.2f;
        #endregion

        [SerializeField]
        Vector2 _moveSpeed = new Vector2(5f, 10f);
        Vector3 _currentSpeed;
        Vector3 _currentDirection;
        Vector3 _targetSpeed;
        [SerializeField]
        float _speedChangeRate = 20f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        [SerializeField]
        float _fallTimeout = 0.15f;
        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        bool _grounded = true;
        [SerializeField]
        float _gravity = -15f;
        float _fallTimeoutDelta;
        [Tooltip("What layers the character uses as ground")]
        [SerializeField]
        LayerMask _groundLayers;
        [SerializeField]
        float _terminalVelocity = 53f;

        bool GroundedCheck(Vector3 origin, out RaycastHit obj)
        {
            Vector3 capsuleBottom = origin - Vector3.up * (_characterController.height / 2);
            Vector3 capsuleTop = origin + Vector3.up * (_characterController.height / 2);

            Vector3 capsuleBottomCenter = capsuleBottom + Vector3.up * _characterController.radius;
            Vector3 capsuleTopCenter = capsuleTop - Vector3.up * _characterController.radius;

            float capsuleCastLength = 0.1f;
            obj = default;
            bool collide = false;
            if (Physics.CapsuleCast(capsuleBottomCenter, capsuleTopCenter, _characterController.radius, Vector3.down, out RaycastHit hit, capsuleCastLength,_groundLayers))
            {
                obj = hit;
                collide =  true;
            }
            var color = collide ? Color.red : Color.cyan;
            DebugExtension.DebugCapsule(capsuleBottom, capsuleTop, radius: _characterController.radius + 0.05f, color: color);
            return collide;
        }

        public void ApplyGrounded()
        {
            _grounded = GroundedCheck(_characterController.transform.position, out var _);
        }

        public void ApplyGravity()
        {
            if (_grounded)
            {
                _fallTimeoutDelta = _fallTimeout;

                if (_targetSpeed.y < 0.0f)
                {
                    _targetSpeed.y = -2f; // 바닥에 붙이기 위한 작은 음수
                }
            }
            else
            {
                if (_targetSpeed.y < _terminalVelocity)
                {
                    _targetSpeed.y += _gravity * Time.deltaTime;
                }
            } 
        }

        public void ApplyInput()
        {
            var moveInput = MoveInput;


            if (_characterController == null)
            {
                Debug.LogError("CharacterController is null.");
                return;
            }

            // 목표 속도 설정
            Vector3 targetVelocity = moveInput == Vector2.zero
                ? Vector3.zero
                : new Vector3(_moveSpeed.x * moveInput.x, 0, _moveSpeed.y * moveInput.y);

            float speedOffset = 0.1f;

            // 속도 보간
            if ((_currentSpeed - targetVelocity).magnitude > speedOffset)
            {
                _currentSpeed = Vector3.Lerp(_currentSpeed, targetVelocity, Mathf.Clamp01(Time.deltaTime * _speedChangeRate));
            }
            else
            {
                _currentSpeed = targetVelocity;
            }

            // 회전 처리
            float targetRoll = -moveInput.x * _maxRollAngle;
            float currentRoll = _characterController.transform.eulerAngles.z;
            float smoothedRoll = Mathf.LerpAngle(currentRoll, targetRoll, Mathf.Clamp01(Time.deltaTime * _rollSensitivity));

            float targetPitch = -moveInput.y * _maxPitchAngle;
            float currentPitch = _characterController.transform.eulerAngles.x;
            float smoothedPitch = Mathf.LerpAngle(currentPitch, targetPitch, Mathf.Clamp01(Time.deltaTime * _pitchSensitivity));

            // 방향만 카메라의 Y축에 맞춰줌
            Quaternion targetRotation = Quaternion.Euler(smoothedPitch, _playerCamera.transform.eulerAngles.y, smoothedRoll);
            _characterController.transform.rotation = targetRotation;

            // 이동 방향 설정 (카메라 기준)
            Vector3 camForward = _playerCamera.transform.forward;
            Vector3 camRight = _playerCamera.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            Vector3 moveDirection = (camForward.normalized * moveInput.y + camRight.normalized * moveInput.x).normalized;
            _currentDirection = Vector3.Lerp(_currentDirection, moveDirection, Mathf.Clamp01(Time.deltaTime * _speedChangeRate));

            Vector3 finalMove = _currentDirection * _currentSpeed.magnitude * Time.deltaTime;
            finalMove.y = _currentSpeed.y * Time.deltaTime;
            _characterController.Move(finalMove);
        }


        private void LateUpdate()
        {
            CameraRotation();
            LookInput = Vector2.zero;
        }
        private void CameraRotation()
        {
            if (LookInput.sqrMagnitude >= CameraThreshold)
            {
                _cinemachineTargetYaw += LookInput.x * cameraYawSensitivity;
                _cinemachineTargetPitch -= LookInput.y * cameraPitchSensitivity;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            _playerCamera.Follow.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
        }



        //        // // 점프 입력 처리
        //        // if (InputManager.Instance.jump && _jumpTimeoutDelta <= 0.0f)
        //        // {
        //        //    _verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
        //        // }

        //        // // 점프 쿨다운 감소
        //        // if (_jumpTimeoutDelta >= 0.0f)
        //        // {
        //        //    _jumpTimeoutDelta -= Time.deltaTime;
        //        // }
        //    }
        //    else
        //    {
        //        // // 공중에 있는 경우 점프 쿨다운 초기화
        //        // _jumpTimeoutDelta = jumpTimeout;

        //        // 중력 적용
        //        if (_verticalSpeed < _terminalVelocity)
        //        {
        //            _verticalSpeed += _gravity * Time.deltaTime;
        //        }

        //        // // 공중에서는 점프 입력 무시
        //        // InputManager.Instance.jump = false;
        //    }
        //}


        public void Initialize(CharacterController characterConotroller , TickController tickContoller )
        {
            _fsm = new MoveFSM<MoveController>(this);
            _characterController = characterConotroller;
            _tickContoller = tickContoller;
            inputActions = InputSingleton.Singleton.playerInput.actions.FindActionMap("Player");
            BindInput();
        }
        public void Update()
        {
            _fsm.OnUpdate();
        }
        private float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        public Vector2 MoveInput { get; set; }
        public Vector2 LookInput { get; set; }
        public bool JumpInput { get; set; }
        public bool AttackInput { get; set; }
        public bool SkillInput { get; set; }
        public void BindInput()
        {
            inputActions["Move"].performed += OnMove;
            inputActions["Look"].performed += OnLook;
            inputActions["Attack"].performed += OnAttack;
            inputActions["Skill"].performed += OnSkill;
            inputActions["Jump"].performed += OnJump;
            inputActions.Enable();
        }
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            Debug.Log(MoveInput);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            AttackInput = context.ReadValue<float>() == 1;
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            JumpInput = context.ReadValue<float>() == 1;
        }
        public void OnSkill(InputAction.CallbackContext context)
        {
            SkillInput = context.ReadValue<float>() == 1;
        }
    }
}