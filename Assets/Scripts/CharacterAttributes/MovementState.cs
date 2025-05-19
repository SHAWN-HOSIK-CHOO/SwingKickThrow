using Unity.Netcode;
using UnityEngine;

namespace CharacterAttributes
{
   public class MovementState : BaseState
   {
      private CharacterController _characterController;
      private Transform           _cameraTransform;
      private float               _moveSpeed;
      private float               _speedChangeRate;
      private float               _speed;
      private float               _verticalSpeed;
      private Vector3             _currentDirection = Vector3.zero;
      private float               _maxRollAngle;
      private float               _maxPitchAngle;
      private float               _rollSensitivity;
      private float               _pitchSensitivity;
      private float               _groundOffset;
      private float               _gravity;
      private float               _fallTimeout;
      private bool                _grounded;
      private float               _groundedRadius;
      private LayerMask           _groundLayers;
      private float               _terminalVelocity;
      private float               _originOffset;
      private float               _rayOffset;
      private float               _rayLength;
      
      private float _fallTimeoutDelta;
      

      public override void Setup(CharacterFSMContext context, CharacterControllerNetwork owner)
      {
         base.Setup(context, owner);
         _characterController = Owner.characterController;
         _cameraTransform     = Owner.mainCamera.transform;
         _moveSpeed           = Owner.characterStatusNetwork.speed;
         _speedChangeRate     = Owner.speedChangeRate;
         _maxRollAngle        = Owner.maxRollAngle;
         _rollSensitivity     = Owner.rollSensitivity;
         _maxPitchAngle       = Owner.maxPitchAngle;
         _pitchSensitivity    = Owner.pitchSensitivity;
         _groundOffset        = Owner.groundOffset;
         _gravity             = Owner.gravity;
         _fallTimeout         = Owner.fallTimeout;
         _grounded            = Owner.grounded;
         _groundedRadius      = Owner.groundedRadius;
         _groundLayers        = Owner.groundLayers;
         _terminalVelocity    = Owner.terminalVelocity;
         _rayOffset           = Owner.rayOffset;
         _rayLength           = Owner.rayLength;
         _originOffset        = Owner.originOffset;
         
         _fallTimeoutDelta = _fallTimeout;
      }

      public override void OnEnter()
      {
            _moveSpeed = Owner.characterStatusNetwork.speed;
            Owner.dashTimer.StartCharging();

            if (Owner.IsOwner)
            {
                Owner.playerInput.enabled = true;
            }
        }
      
      public override void OnUpdate()
      {
         Gravity();
         GroundedCheck();
         Move();
      }

      public override void OnExit()
      {
         Owner.dashTimer.StopCharging();
      }

      private void Move()
      {
         if (_characterController == null)
         {
            Debug.Log("CC is null");
         }
         
         float   targetSpeed = Owner.characterStatusNetwork.speed;
         Vector2 move        = Owner.MoveInput;
         if (move == Vector2.zero)
         {
            targetSpeed = 0f;
         }

         if (!Owner.dashTimer.IsPaused && _characterController.velocity.magnitude == 0f)
         {
            Owner.dashTimer.StopCharging();
         }

         if (Owner.dashTimer.IsPaused && _characterController.velocity.magnitude != 0f)
         {
            Owner.dashTimer.StartCharging();
         }
         
         float currentHorizontalSpeed =
            new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z).magnitude;
         float speedOffset = 0.1f;

         if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
         {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * _speedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
         }
         else
         {
            _speed = targetSpeed;
         }
         
         Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;
         Vector3 moveDirection = Vector3.zero;

         if (move != Vector2.zero)
         {
            Vector3 camForward = _cameraTransform.forward;
            Vector3 camRight   = _cameraTransform.right;
            camForward.y = 0f;
            camRight.y   = 0f;
            camForward.Normalize();
            camRight.Normalize();

            moveDirection = camForward * inputDirection.z + camRight * inputDirection.x;
            moveDirection.Normalize();
         }
         
         _currentDirection = Vector3.Lerp(_currentDirection, moveDirection, Time.deltaTime * _speedChangeRate);

         _characterController.Move(_currentDirection                       * (_speed * Time.deltaTime) +
                                   new Vector3(0.0f, _verticalSpeed, 0.0f) * Time.deltaTime);

         float targetRoll = -move.x * _maxRollAngle;
         float currentRoll  = Owner.transform.eulerAngles.z;
         float smoothedRoll = Mathf.LerpAngle(currentRoll, targetRoll, Time.deltaTime * _rollSensitivity);
         
         float targetPitch   = -move.y * _maxPitchAngle;
         float currentPitch  = Owner.transform.eulerAngles.x;
         float smoothedPitch = Mathf.LerpAngle(currentPitch, targetPitch, Time.deltaTime * _pitchSensitivity);
         
         Owner.transform.rotation = Quaternion.Euler(smoothedPitch, _cameraTransform.eulerAngles.y, smoothedRoll);
      }
      
      private void GroundedCheck()
      {
         Vector3 spherePosition = new Vector3(Owner.transform.position.x, Owner.transform.position.y - _originOffset - _groundOffset,
                                              Owner.transform.position.z);
         
         _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers,
                                        QueryTriggerInteraction.Ignore);

         // 디버깅용 구체 그리기
         Debug.DrawLine(Owner.transform.position, spherePosition, _grounded ? Color.blue : Color.red);
      }

      private void Gravity()
      {
         if (_grounded)
         {
            _fallTimeoutDelta = _fallTimeout;

            if (_verticalSpeed < 0.0f)
            {
               _verticalSpeed = -2f; // 바닥에 붙이기 위한 작은 음수
            }

            // // 점프 입력 처리
            // if (InputManager.Instance.jump && _jumpTimeoutDelta <= 0.0f)
            // {
            //    _verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            // }

            // // 점프 쿨다운 감소
            // if (_jumpTimeoutDelta >= 0.0f)
            // {
            //    _jumpTimeoutDelta -= Time.deltaTime;
            // }
         }
         else
         {
            // // 공중에 있는 경우 점프 쿨다운 초기화
            // _jumpTimeoutDelta = jumpTimeout;

            // 중력 적용
            if (_verticalSpeed < _terminalVelocity)
            {
               _verticalSpeed += _gravity * Time.deltaTime;
            }

            // // 공중에서는 점프 입력 무시
            // InputManager.Instance.jump = false;
         }
      }
   }
}
