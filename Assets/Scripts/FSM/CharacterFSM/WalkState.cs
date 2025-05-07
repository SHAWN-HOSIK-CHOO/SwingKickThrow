using CharacterAttributes;
using FSM;
using UnityEngine;
namespace Character.MoveFSM
{
    public class WalkState<OwnerType> : BaseState<CharacterState, OwnerType> where OwnerType : CharacterControllerNetwork
    {
        CharacterControllerNetwork _controller;
      
        override protected void OnEnter()
        {
            _controller = _owner.Get;
        }

        override protected void OnUpdate()
        {
            MovementState state = (MovementState)_controller._fsm.statePool[FSMType.MOVEMENT];
            state.Gravity();
            state.GroundedCheck();
            state.Move();

        }
        override protected void OnExit()
        {

        }
    }
}