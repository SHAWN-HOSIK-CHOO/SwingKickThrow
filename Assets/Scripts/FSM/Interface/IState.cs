using CharacterAttributes;
using UnityEngine;

namespace FSM
{
    public interface IState<StateEnum,OwnerType>
    {
        IFSM<StateEnum, OwnerType> upperFSM { get; }
        IOwner<OwnerType> owner { get; }

        void Setup(IFSM<StateEnum, OwnerType> fsm);
        void Setup(IOwner<OwnerType> owner);
        void OnEnter();
        void OnUpdate();
        void OnExit();
    }
}