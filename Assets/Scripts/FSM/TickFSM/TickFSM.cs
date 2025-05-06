
using Character;
using FSM;

namespace TickSystem
{
    public enum TickState
    {
        Render,
        Simul
    }
    public class T_Render_Simul : BsaeTanstition<TickState>
    {
        public override TickState _before => TickState.Render;
        public override TickState _after => TickState.Simul;
    }
    public class T_Simul_Render : BsaeTanstition<TickState>
    {
        public override TickState _before => TickState.Simul;
        public override TickState _after => TickState.Render;
    }
    public class TickFSM<OwnerType>  : BaseFSM<TickState, OwnerType> where OwnerType : TickController
    {
        IFSM<TickState, OwnerType> _fsm;
        public TickFSM(IOwner<OwnerType> controller)
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
            _fsm.TriggerTransition(TickState.Render, true);
        }
        void AddTransition()
        {
            ITransition<TickState> render_simul = new T_Render_Simul();
            _fsm.AddTransition(render_simul);

            ITransition<TickState> simul_render = new T_Simul_Render();
            _fsm.AddTransition(simul_render);
        }

        void AddState(IOwner<OwnerType> controller)
        {
            IFSM<TickState, OwnerType> fsm = this;

            FSM.IState<TickState, OwnerType> renderState = new TickFSM.RenderState<OwnerType>();
            renderState.Setup(controller);
            fsm.AddState(TickState.Render, renderState);

            FSM.IState<TickState, OwnerType> simulState = new TickFSM.SimulState<OwnerType>();
            simulState.Setup(controller);
            fsm.AddState(TickState.Simul, simulState);
        }
        void AddCondition()
        {
            
        }
    }
}