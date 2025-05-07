using FSM;
using UnityEngine;
namespace Character.MoveFSM
{
    public class IdleState<OwnerType> : BaseState<MoveState, OwnerType> where OwnerType : MoveController
    {
        MoveController _controller;
      
        override protected void OnEnter()
        {
            _controller = _owner.Get;
        }

        override protected void OnUpdate()
        {
            _controller.ApplyGravity();
            _controller.ApplyGrounded();
            _controller.ApplyInput();
        }
        override protected void OnExit()
        {

        }
    }
}