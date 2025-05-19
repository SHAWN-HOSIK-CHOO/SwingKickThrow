using Character;
using UnityEngine;
namespace FSM
{
    public class BaseState<StateEnum, OwnerType> : FSM.IState<StateEnum, OwnerType>
    {
        public IFSM<StateEnum, OwnerType> _upperFSM { get; protected set; }
        public IOwner<OwnerType> _owner { get; protected set; }

        IFSM<StateEnum, OwnerType> IState<StateEnum, OwnerType>.upperFSM => _upperFSM;

        IOwner<OwnerType> IState<StateEnum, OwnerType>.owner => _owner;

        void IState<StateEnum, OwnerType>.OnEnter()
        {
            OnEnter();
        }


        void IState<StateEnum, OwnerType>.OnExit()
        {
            OnExit();
        }
        void IState<StateEnum, OwnerType>.OnUpdate()
        {
            OnUpdate();
        }
        virtual protected void OnEnter()
        {

        }
        virtual protected void OnExit()
        {

        }

        virtual protected void OnUpdate()
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