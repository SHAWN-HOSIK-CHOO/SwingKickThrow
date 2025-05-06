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
        Jump, 
        Dash,
    }

    class T_Any_IDLE : BsaeTanstition<MoveState>
    {
        public override MoveState _before => MoveState.Any;
        public override MoveState _after => MoveState.IDLE;
    }
    class T_IDLE_Jump : BsaeTanstition<MoveState>
    {
        public override MoveState _before => MoveState.IDLE;
        public override MoveState _after => MoveState.Jump;
    }
    class T_Jump_IDLE : BsaeTanstition<MoveState>
    {
        public override MoveState _before => MoveState.Jump;
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
    class T_Jump_Dash : BsaeTanstition<MoveState>
    {
        public override MoveState _before => MoveState.Jump;
        public override MoveState _after => MoveState.Dash;
    }
    class T_Dash_Jump : BsaeTanstition<MoveState>
    {
        public override MoveState _before => MoveState.Dash;
        public override MoveState _after => MoveState.Jump;
    }

    public class MoveFSM<OwnerType>  : BaseFSM<MoveState, OwnerType> where OwnerType : MoveController 
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
            _fsm.TriggerTransition(MoveState.Any,true);
        }
        void AddTransition()
        {
            ITransition<MoveState> any_idle = new T_Any_IDLE();
            _fsm.AddTransition(any_idle);

            ITransition<MoveState> idle_dash = new T_IDLE_Dash();
            _fsm.AddTransition(idle_dash);

            ITransition<MoveState> dash_idle = new T_Dash_IDLE();
            _fsm.AddTransition(dash_idle);

            ITransition<MoveState> idle_jump = new T_IDLE_Jump();
            _fsm.AddTransition(idle_jump);

            ITransition<MoveState> jump_idle = new T_Jump_IDLE();
            _fsm.AddTransition(jump_idle);

            ITransition<MoveState> jump_dash = new T_Jump_Dash();
            _fsm.AddTransition(jump_dash);

            ITransition<MoveState> dash_jump = new T_Dash_Jump();
            _fsm.AddTransition(dash_jump);
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


            FSM.IState<MoveState, OwnerType> jumpState = new Character.MoveFSM.JumpState<OwnerType>();
            jumpState.Setup(controller);
            fsm.AddState(MoveState.Jump, jumpState);


            FSM.IState<MoveState, OwnerType> dashState = new Character.MoveFSM.DashState<OwnerType>();
            dashState.Setup(controller);
            fsm.AddState(MoveState.Dash, dashState);
        }
        void AddCondition()
        {
            
        }
    }
}