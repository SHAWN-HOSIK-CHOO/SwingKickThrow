using UnityEngine;

namespace CharacterAttributes
{
   public class AttackState : BaseState
   {
      private Animator _animator;
      private int      _animaIDAttack;

      public override void Setup(CharacterFSMContext context, CharacterControllerNetwork owner)
      {
         base.Setup(context, owner);
         _animator      = Owner.animator;
         
         _animaIDAttack = Animator.StringToHash("Attack");
      }

      public override void OnEnter()
      {
         _animator.SetBool(_animaIDAttack,true);
      }

      public override void OnUpdate()
      {
         //Debug.Log("Called Update from AttackState");
         var stateInfo = _animator.GetCurrentAnimatorStateInfo(0); 
         if (stateInfo.IsName("MooseThrowAction") && stateInfo.normalizedTime >= 1f)
         {
            //Debug.Log("Called Finish from AttackState");
            Finish(FSMType.ATTACK);
         }
      }

      public override void OnExit()
      {
         _animator.SetBool(_animaIDAttack,false);
      }
   }
}
