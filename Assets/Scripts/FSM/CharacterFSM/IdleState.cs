using CharacterAttributes;
using FSM;
using UnityEngine;
namespace Character.MoveFSM
{
    public class IdleState<OwnerType> : BaseState<CharacterState, OwnerType> where OwnerType : CharacterControllerNetwork
    {
        CharacterControllerNetwork _controller;
        override protected void OnEnter()
        {
            _controller = _owner.Get;
        }

        override protected void OnUpdate()
        {
            //TO DO : 캐스팅 보다는 MovementState의 인터페이스를 만들거나 MovementState를 OwnerType으로 하는 FSM을 정의하는것이 나을듯
            MovementState state = (MovementState)_controller._fsm.statePool[FSMType.MOVEMENT];
            state.Gravity();
            state.GroundedCheck();
            //TO DO : Idle 상태에서 Move라는 이름을 쓰는게 어색한가? 기능 분리해도 될 듯
            state.Move(); 
        }
        override protected void OnExit()
        {

        }
    }
}