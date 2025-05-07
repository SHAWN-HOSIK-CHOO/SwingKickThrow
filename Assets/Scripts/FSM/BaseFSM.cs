using Character;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public abstract class BaseFSM<StateEnum, OwnerType> : BaseState<StateEnum, OwnerType>, IFSM<StateEnum, OwnerType>
    {
        StateEnum _currentState;
        StateEnum IFSM<StateEnum, OwnerType>.currentState => _currentState;

        Dictionary<StateEnum, IState<StateEnum, OwnerType>> _states = new();
        DoubleKeyDict<StateEnum, StateEnum, ITransition<StateEnum>> _transitions = new();

        void IFSM<StateEnum, OwnerType>.AddState(StateEnum type, IState<StateEnum, OwnerType> state)
        {
            state.Setup(this);  
            _states.TryAdd(type, state);
        }

        void IFSM<StateEnum, OwnerType>.AddTransition(ITransition<StateEnum> transition)
        {
            _transitions.TryAdd(transition.before, transition.after, transition);
        }
        IState<StateEnum, OwnerType> IFSM<StateEnum, OwnerType>.GetState(StateEnum type)
        {
            return _states[type];
        }

        void Transition(StateEnum nextState)
        {
            IFSM<StateEnum, OwnerType> ifsm = this;
            ifsm.GetState(_currentState).OnExit();
            _currentState = nextState;
            ifsm.GetState(_currentState).OnEnter();
        }

        void IFSM<StateEnum, OwnerType>.TriggerTransition(StateEnum nextState, bool force)
        {
            if(_transitions.TryGetValue(_currentState, nextState, out var transition))
            {
                if(force)
                {
                    Transition(nextState);
                }
                else
                {
                    if(transition.CheckCondition())
                    {
                        Transition(nextState);
                    }
                }
            }

        }


        protected override void OnEnter() 
        { 
        
        }

        protected override void OnUpdate()
        {

            IFSM<StateEnum, OwnerType> fsm = this;
            fsm.GetState(_currentState).OnUpdate();
        }

        protected override void OnExit()
        {

        }

        public ITransition<StateEnum> GetTransition(StateEnum before, StateEnum after)
        {
            _transitions.TryGetValue(before, after, out var val);
            return val;
        }
    }
}