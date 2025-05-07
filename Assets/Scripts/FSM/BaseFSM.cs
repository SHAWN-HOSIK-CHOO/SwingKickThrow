using Character;
using System;
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
            if(_states.TryGetValue(type,out var val))
            {
                return val;
            }

            return null;
        }

        void Transition(StateEnum nextState)
        {
            IFSM<StateEnum, OwnerType> ifsm = this;
            IState<StateEnum, OwnerType> currentState = ifsm.GetState(_currentState);
            currentState?.OnExit();
            _currentState = nextState;
            currentState = ifsm.GetState(_currentState);
            currentState?.OnEnter();
        }

        void IFSM<StateEnum, OwnerType>.TriggerTransition(StateEnum nextState, bool force)
        {
            if(_currentState.Equals(nextState))
            {
                return;
            }

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
            else if (force)
            {
                Transition(nextState);
            }
        }
        protected override void OnEnter() 
        { 
        
        }

        protected override void OnUpdate()
        {
            IFSM<StateEnum, OwnerType> fsm = this;
            IState<StateEnum, OwnerType> currentState = fsm.GetState(_currentState);
            currentState?.OnUpdate();
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