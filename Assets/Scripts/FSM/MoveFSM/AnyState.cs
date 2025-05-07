using Character;
using CharacterAttributes;
using FSM;

namespace Character.MoveFSM
{
    public class AnyState<OwnerType> : BaseState<MoveState, OwnerType> where OwnerType : MovementState
    {
        MovementState _controller;
        override protected void OnEnter()
        {
            _controller = _owner.Get;
        }

        override protected void OnUpdate()
        {
            _upperFSM.TriggerTransition(MoveState.IDLE);
        }
        override protected void OnExit()
        {

        }
    }
}