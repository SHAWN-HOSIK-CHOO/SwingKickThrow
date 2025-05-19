using CharacterAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace FSM
{
    /*
     * State를 정의후 StateEnum으로 넣을 것
     * 필요하다면 AnyState를 Transitoin에 추가하여 상태전이를 용이하게 할 것
     * 필요하다면 Transition 단계도 State로서 추가할것. 
     */

    public interface IFSM<StateEnum, OwnerType> : IState<StateEnum, OwnerType>
    {
        StateEnum currentState { get;}
        IState<StateEnum, OwnerType> GetState(StateEnum type);

        ITransition<StateEnum> GetTransition(StateEnum before, StateEnum after);

        void AddState(StateEnum type, IState<StateEnum, OwnerType> state);
        void AddTransition(ITransition<StateEnum> transition);
        void TriggerTransition(StateEnum nextState , bool force = false);
    }
}