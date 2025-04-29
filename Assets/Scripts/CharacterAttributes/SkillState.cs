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

      private float _dashTimeRemaining;
      
      public override void Setup(CharacterFSMContext context, CharacterControllerNetwork owner)
      {
         base.Setup(context, owner);
         _characterController = Owner.characterController;
      }

      public override void OnEnter()
      {
         _forwardVector     = Owner.transform.forward;
         _dashDuration      = Owner.dashDuration;
         _dashSpeed         = Owner.dashSpeed;
         _bounceBackForce   = Owner.bounceBackForce;
         _dashTimeRemaining = _dashDuration;
         
         _forwardVector.Normalize();
      }

      public override void OnUpdate()
      {
         if (_dashTimeRemaining >= 0f)
         {
            Vector3        move  = _forwardVector * ( _dashSpeed * Time.deltaTime );
            CollisionFlags flags = _characterController.Move(move);
            
            if ((flags & CollisionFlags.Sides) != 0)
            {
               //Debug.Log("Collision detected, bouncing back");
               Vector3 collisionNormal = -_forwardVector; // 충돌 방향 반대
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
