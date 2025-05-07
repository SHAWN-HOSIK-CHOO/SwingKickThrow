using CharacterAttributes;
using FSM;
using UnityEngine;
namespace Character.MoveFSM
{
    public class HitState<OwnerType> : BaseState<CharacterState, OwnerType> where OwnerType : CharacterControllerNetwork
    {
        CharacterControllerNetwork _controller;
        override protected void OnEnter()
        {
            _controller = _owner.Get;
        }

        override protected void OnUpdate()
        {
            HitState state = (HitState)_controller._fsm.statePool[FSMType.HIT];
            state.UpdateHit();
        }
        override protected void OnExit()
        {

        }
    }
}