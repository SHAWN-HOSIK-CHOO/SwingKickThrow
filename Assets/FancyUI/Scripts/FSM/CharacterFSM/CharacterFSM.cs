using CharacterAttributes;
using FSM;

namespace Character
{
    public enum CharacterState
    {
        Any,
        IDLE,
        Walk, 
        Dash,
        Hit,
    }

    class T_Any_IDLE : BsaeTanstition<CharacterState>
    {
        public override CharacterState _before => CharacterState.Any;
        public override CharacterState _after => CharacterState.IDLE;
    }
    class T_Hit : BsaeTanstition<CharacterState>
    {
        public override CharacterState _before => CharacterState.Any;
        public override CharacterState _after => CharacterState.Hit;
    }

    class T_IDLE_Walk : BsaeTanstition<CharacterState>
    {
        public override CharacterState _before => CharacterState.IDLE;
        public override CharacterState _after => CharacterState.Walk;
    }
    class T_Walk_IDLE : BsaeTanstition<CharacterState>
    {
        public override CharacterState _before => CharacterState.Walk;
        public override CharacterState _after => CharacterState.IDLE;
    }
    class T_IDLE_Dash : BsaeTanstition<CharacterState>
    {
        public override CharacterState _before => CharacterState.IDLE;
        public override CharacterState _after => CharacterState.Dash;
    }
    class T_Dash_IDLE : BsaeTanstition<CharacterState>
    {
        public override CharacterState _before => CharacterState.Dash;
        public override CharacterState _after => CharacterState.IDLE;
    }

    class T_Dash_Walk : BsaeTanstition<CharacterState>
    {
        public override CharacterState _before => CharacterState.Dash;
        public override CharacterState _after => CharacterState.Walk;
    }
    class T_Walk_Dash : BsaeTanstition<CharacterState>
    {
        public override CharacterState _before => CharacterState.Walk;
        public override CharacterState _after => CharacterState.Dash;
    }
    public class CharacterFSM<OwnerType>  : BaseFSM<CharacterState, OwnerType> where OwnerType : CharacterControllerNetwork
    {
        IFSM<CharacterState, OwnerType> _fsm;
        public CharacterFSM(IOwner<OwnerType> controller)
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
            _fsm.TriggerTransition(CharacterState.Any, true);
        }
        void AddTransition()
        {
            ITransition<CharacterState> any_idle = new T_Any_IDLE();
            _fsm.AddTransition(any_idle);

            ITransition<CharacterState> idle_walk = new T_IDLE_Walk();
            _fsm.AddTransition(idle_walk);

            ITransition<CharacterState> walk_idle = new T_Walk_IDLE();
            _fsm.AddTransition(walk_idle);

            ITransition<CharacterState> idle_dash = new T_IDLE_Dash();
            _fsm.AddTransition(idle_dash);

            ITransition<CharacterState> dash_idle = new T_Dash_IDLE();
            _fsm.AddTransition(dash_idle);

            ITransition<CharacterState> walk_dash = new T_Walk_Dash();
            _fsm.AddTransition(walk_dash);

            ITransition<CharacterState> dash_walk = new T_Dash_Walk();
            _fsm.AddTransition(dash_walk);

            ITransition<CharacterState> hit = new T_Hit();
            _fsm.AddTransition(hit);
        }
        void AddState(IOwner<OwnerType> controller)
        {
            IFSM<CharacterState, OwnerType> fsm = this;

            FSM.IState<CharacterState, OwnerType> anyState = new Character.MoveFSM.AnyState<OwnerType>();
            anyState.Setup(controller);
            fsm.AddState(CharacterState.Any, anyState);

            FSM.IState<CharacterState, OwnerType> idleState = new Character.MoveFSM.IdleState<OwnerType>();
            idleState.Setup(controller);
            fsm.AddState(CharacterState.IDLE, idleState);

            FSM.IState<CharacterState, OwnerType> walkState = new Character.MoveFSM.WalkState<OwnerType>();
            walkState.Setup(controller);
            fsm.AddState(CharacterState.Walk, walkState);

            FSM.IState<CharacterState, OwnerType> dashState = new Character.MoveFSM.DashState<OwnerType>();
            dashState.Setup(controller);
            fsm.AddState(CharacterState.Dash, dashState);

            FSM.IState<CharacterState, OwnerType> hitState = new Character.MoveFSM.HitState<OwnerType>();
            hitState.Setup(controller);
            fsm.AddState(CharacterState.Dash, hitState);


        }
        void AddCondition()
        {
            //To do : add Transition Condition
        }
    }
}