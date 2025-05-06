using Character;
using FSM;
using UnityEngine;
using UnityEngine.UIElements;

namespace Character.MoveFSM
{
    public class AnyState : FSM.IState<MoveState, Controller>
    {
        IFSM<MoveState, Controller> _upperFSM;

        IFSM<MoveState, Controller> IState<MoveState, Controller>._upperFSM => _upperFSM;
        IOwner<Controller> _owner;
        IOwner<Controller> IState<MoveState, Controller>._owner => _owner;

        void IState<MoveState, Controller>.Setup(IOwner<Controller> owner)
        {
            _owner = owner;
        }

        void IState<MoveState, Controller>.OnEnter()
        {
            throw new System.NotImplementedException();
        }

        void IState<MoveState, Controller>.OnExit()
        {
            throw new System.NotImplementedException();
        }

        void IState<MoveState, Controller>.OnUpdate()
        {
            throw new System.NotImplementedException();
        }

        void IState<MoveState, Controller>.Setup(IFSM<MoveState, Controller> fsm)
        {
            throw new System.NotImplementedException();
        }

    }
}