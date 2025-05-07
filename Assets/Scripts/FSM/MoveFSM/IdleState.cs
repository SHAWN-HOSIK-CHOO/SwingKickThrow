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


        }
        override protected void OnExit()
        {

        }
    }
}