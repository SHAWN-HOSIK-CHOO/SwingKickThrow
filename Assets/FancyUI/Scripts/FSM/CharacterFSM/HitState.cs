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

        }
        override protected void OnExit()
        {

        }
    }
}