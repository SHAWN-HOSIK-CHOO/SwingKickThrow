using CharacterAttributes;
using FSM;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Character
{
    public enum MoveState
    {
        AnyState,
        IDLE,
        Jump,
        Dash,
    }

    public class MoveFSM<T>  : FSM<MoveState,T>
    {
        public MoveFSM(IOwner<T> controller)
        {
            IFSM<MoveState,T> fsm = this;
            fsm.Setup(controller);
            //fsm.AddState(MoveState.IDLE, new  AnyState<MoveState>());
        }
    }
}