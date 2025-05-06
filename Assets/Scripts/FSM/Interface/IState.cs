using CharacterAttributes;
using UnityEngine;

namespace FSM
{
    public interface IState<StateEnum,OwnerType>
    {
        IFSM<StateEnum, OwnerType> _upperFSM { get; }
        IOwner<OwnerType> _owner { get; }

        void Setup(IFSM<StateEnum, OwnerType> fsm);
        void Setup(IOwner<OwnerType> owner);
        void OnEnter();
        void OnUpdate();
        void OnExit();
    }
}