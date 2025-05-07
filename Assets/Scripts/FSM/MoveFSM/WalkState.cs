using CharacterAttributes;
using FSM;
using UnityEngine;
namespace Character.MoveFSM
{
    public class WalkState<OwnerType> : BaseState<MoveState, OwnerType> where OwnerType : MovementState
    {
        MovementState _controller;
      
        override protected void OnEnter()
        {
            _controller = _owner.Get;
        }

        override protected void OnUpdate()
        {
            _controller.Gravity();
            _controller.GroundedCheck();
            _controller.Move();

        }
        override protected void OnExit()
        {

        }
    }
}