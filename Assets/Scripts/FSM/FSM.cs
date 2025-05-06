using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public abstract class FSM<StateEnum, OwnerType> : IFSM<StateEnum, OwnerType>
    {
        StateEnum _currentState;
        IFSM<StateEnum, OwnerType> _upperFSM;

        StateEnum IFSM<StateEnum, OwnerType>.currentState => _currentState;
        IFSM<StateEnum, OwnerType> IState<StateEnum, OwnerType>._upperFSM => _upperFSM;
        IOwner<OwnerType> _owner;
        IOwner<OwnerType> IState<StateEnum, OwnerType>._owner => _owner;


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

        void IState<StateEnum, OwnerType>.OnEnter()
        {
            OnEnter();
        }

        protected virtual void OnEnter() 
        { 
        
        }

        void IState<StateEnum, OwnerType>.OnUpdate()
        {
            OnUpdate();
        }
        protected virtual void OnUpdate()
        {

        }

        void IState<StateEnum, OwnerType>.OnExit()
        {
            OnExit();
        }
        protected virtual void OnExit()
        {

        }

        void IState<StateEnum, OwnerType>.Setup(IFSM<StateEnum, OwnerType> fsm)
        {
            _upperFSM = fsm;
        }

        void IState<StateEnum, OwnerType>.Setup(IOwner<OwnerType> owner)
        {
            _owner = owner;
        }
    }
}