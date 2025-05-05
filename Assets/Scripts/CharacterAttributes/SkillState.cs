using Unity.Netcode;
using UnityEngine;

namespace CharacterAttributes
{
   public class SkillState : BaseState
   {
      private CharacterController _characterController;
      private Vector3             _forwardVector;
      private float               _dashDuration;
      private float               _dashSpeed;
      private float               _bounceBackForce;
      private AnimationCurve      _animationCurve;

      private float _dashTimeRemaining;
      
      private float _currentSpeed;
      private float _rotationSpeed = 6f;
      //private float _acceleration = 20f;
      
      public override void Setup(CharacterFSMContext context, CharacterControllerNetwork owner)
      {
         base.Setup(context, owner);
         _characterController = Owner.characterController;
         _animationCurve      = Owner.dashCurve;
      }

      public override void OnEnter()
      {
         _forwardVector     = Owner.transform.forward;
         _dashDuration      = Owner.dashDuration;
         _dashSpeed         = Owner.dashSpeed;
         _bounceBackForce   = Owner.bounceBackForce;
         _dashTimeRemaining = _dashDuration;
         
         _forwardVector.Normalize();

         if (Owner.characterStatusNetwork == null)
         {
            Debug.Log("---------------STATUS_NULL--------------------");
         }

         _currentSpeed     = 0f;
         Owner.crashFactor = _dashSpeed;
      }
      
      public override void OnUpdate()
      {
         if (_dashTimeRemaining >= 0f)
         {
            float progress    = 1f - (_dashTimeRemaining / _dashDuration);
            float speedFactor = _animationCurve.Evaluate(progress);
            
            _currentSpeed = _dashSpeed * speedFactor;

            float yaw = Owner.LookInput.x * _rotationSpeed * Time.deltaTime;
            Owner.transform.Rotate(Vector3.up, yaw);
            
            _forwardVector = Owner.transform.forward;
            
            Vector3        move  = _forwardVector * (_currentSpeed * Time.deltaTime);
            CollisionFlags flags = _characterController.Move(move);

            if ((flags & CollisionFlags.Sides) != 0)
            {
               Vector3 collisionNormal = -_forwardVector;
               Vector3 bounceDirection = Vector3.Reflect(_forwardVector, collisionNormal).normalized;

               _characterController.Move(bounceDirection * (_bounceBackForce * Time.deltaTime));
               Finish(FSMType.SKILL);
               return;
            }

            _dashTimeRemaining -= Time.deltaTime;
         }
         else
         {
            Finish(FSMType.SKILL);
         }
      }

      public override void OnExit()
      {
         
      }
   }
}
