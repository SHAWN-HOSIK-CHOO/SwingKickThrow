using CharacterAttributes;
using FSM;
using UnityEngine;
namespace Character.MoveFSM
{
    public class DashState<OwnerType> : BaseState<CharacterState, OwnerType> where OwnerType : CharacterControllerNetwork
    {
        CharacterControllerNetwork _controller;

        override protected void OnEnter()
        {
            _controller = _owner.Get;
        }

        override protected void OnUpdate()
        {
            SkillState state = (SkillState)_controller._fsm.statePool[FSMType.SKILL];
            state.DashUpdate();
        }
        override protected void OnExit()
        {

        }
    }
}