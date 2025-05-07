using Character.MoveFSM;
using CharacterAttributes;
using FSM;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Character
{
    public enum MoveState
    {
        Any,
        IDLE,
        Walk, 
        Dash,
    }

    class T_Any_IDLE : BsaeTanstition<MoveState>
    {
        public override MoveState _before => MoveState.Any;
        public override MoveState _after => MoveState.IDLE;
    }
    class T_IDLE_Walk : BsaeTanstition<MoveState>
    {
        public override MoveState _before => MoveState.IDLE;
        public override MoveState _after => MoveState.Walk;
    }
    class T_Walk_IDLE : BsaeTanstition<MoveState>
    {
        public override MoveState _before => MoveState.Walk;
        public override MoveState _after => MoveState.IDLE;
    }
    class T_IDLE_Dash : BsaeTanstition<MoveState>
    {
        public override MoveState _before => MoveState.IDLE;
        public override MoveState _after => MoveState.Dash;
    }
    class T_Dash_IDLE : BsaeTanstition<MoveState>
    {
        public override MoveState _before => MoveState.Dash;
        public override MoveState _after => MoveState.IDLE;
    }

    class T_Dash_Walk : BsaeTanstition<MoveState>
    {
        public override MoveState _before => MoveState.Dash;
        public override MoveState _after => MoveState.Walk;
    }
    class T_Walk_Dash : BsaeTanstition<MoveState>
    {
        public override MoveState _before => MoveState.Walk;
        public override MoveState _after => MoveState.Dash;
    }

    public class MoveFSM<OwnerType>  : BaseFSM<MoveState, OwnerType> where OwnerType : MovementState
    {
        IFSM<MoveState, OwnerType> _fsm;
        public MoveFSM(IOwner<OwnerType> controller)
        {
            _fsm = this;
            _fsm.Setup(controller);
            AddState(controller);
            SetInitState();
            AddTransition();
            AddCondition();
        }
        void SetInitState()
        {
            _fsm.TriggerTransition(MoveState.Any, true);
        }
        void AddTransition()
        {
            ITransition<MoveState> any_idle = new T_Any_IDLE();
            _fsm.AddTransition(any_idle);

            ITransition<MoveState> idle_walk = new T_IDLE_Walk();
            _fsm.AddTransition(idle_walk);

            ITransition<MoveState> walk_idle = new T_Walk_IDLE();
            _fsm.AddTransition(walk_idle);

            ITransition<MoveState> idle_dash = new T_IDLE_Dash();
            _fsm.AddTransition(idle_dash);

            ITransition<MoveState> dash_idle = new T_Dash_IDLE();
            _fsm.AddTransition(dash_idle);

            ITransition<MoveState> walk_dash = new T_Walk_Dash();
            _fsm.AddTransition(walk_dash);

            ITransition<MoveState> dash_walk = new T_Dash_Walk();
            _fsm.AddTransition(dash_walk);
        }
        void AddState(IOwner<OwnerType> controller)
        {
            IFSM<MoveState, OwnerType> fsm = this;

            FSM.IState<MoveState, OwnerType> anyState = new Character.MoveFSM.AnyState<OwnerType>();
            anyState.Setup(controller);
            fsm.AddState(MoveState.Any, anyState);

            FSM.IState<MoveState, OwnerType> idleState = new Character.MoveFSM.IdleState<OwnerType>();
            idleState.Setup(controller);
            fsm.AddState(MoveState.IDLE, idleState);

            FSM.IState<MoveState, OwnerType> walkState = new Character.MoveFSM.WalkState<OwnerType>();
            idleState.Setup(controller);
            fsm.AddState(MoveState.Walk, walkState);


            FSM.IState<MoveState, OwnerType> dashState = new Character.MoveFSM.DashState<OwnerType>();
            dashState.Setup(controller);
            fsm.AddState(MoveState.Dash, dashState);
        }
        void AddCondition()
        {
            //To do : add Transition Condition
        }
    }
}