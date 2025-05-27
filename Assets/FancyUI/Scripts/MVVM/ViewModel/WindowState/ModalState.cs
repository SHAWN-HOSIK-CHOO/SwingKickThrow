using MVVM.ViewModel.Window.BIND;
using STC;
using static MVVM.ViewModel.Window.State.IDLEState;
namespace MVVM.ViewModel.Window.State
{
    public class ModalState : STC.State
    {
        public const string StateName = "Modal";
        IWindowViewModel Owner;
        public class IsModal : Condition
        {
            IWindowViewModel Owner;
            public IsModal(IWindowViewModel owner) : base(nameof(IsModal),true)
            {
                Owner = owner;
                Owner.IsModal.Subscribe(Callback);
            }
            public override bool OnCheck()
            {
                return Owner.IsModal.Value;
            }
        }
        public class IsNoModal : Condition
        {
            IWindowViewModel Owner;
            public IsNoModal(IWindowViewModel owner) : base(nameof(IsNoModal), true)
            {
                Owner = owner;
                Owner.IsModal.Subscribe(Callback);
            }
            public override bool OnCheck()
            {
                return !Owner.IsModal.Value;
            }
        }
        public ModalState(IWindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState nonInteractiveState = (Owner as IWindowState).NonInteractiveState;
            //NonInteractive -> Modal
            var transition = new STC.Transition(NonInteractiveState.StateName, ModalState.StateName);
            transition.AddCondition(new IsModal(owner));
            nonInteractiveState.AddTransition(transition);
        }

        public override void OnEnter()
        {

        }
        public override void OnExit()
        {

        }
    }
}