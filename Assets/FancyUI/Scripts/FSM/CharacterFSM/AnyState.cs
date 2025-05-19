using Character;
using CharacterAttributes;
using FSM;

namespace Character.MoveFSM
{
    public class AnyState<OwnerType> : BaseState<CharacterState, OwnerType> where OwnerType : CharacterControllerNetwork
    {
        CharacterControllerNetwork _controller;
        override protected void OnEnter()
        {
            _controller = _owner.Get;
        }

        override protected void OnUpdate()
        {
            _upperFSM.TriggerTransition(CharacterState.IDLE);
        }
        override protected void OnExit()
        {

        }
    }
}