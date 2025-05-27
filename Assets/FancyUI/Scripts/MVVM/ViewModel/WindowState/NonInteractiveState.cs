using MVVM.ViewModel.Window.BIND;
using STC;
using static MVVM.ViewModel.Window.State.ModalState;
using static MVVM.ViewModel.Window.State.TransitionState;
namespace MVVM.ViewModel.Window.State
{
    public class NonInteractiveState : STC.State 
    {
        public const string StateName = "NonInteractive";
        IWindowViewModel Owner;
        public class IsNonInteractive : Condition
        {
            IWindowViewModel Owner;
            public IsNonInteractive(IWindowViewModel owner) : base(true)
            {
                Owner = owner;
                Owner.Interactable.Subscribe(Callback);
            }
            public override bool OnCheck()
            {
                return !Owner.Interactable.Value;
            }
        }
        public NonInteractiveState(IWindowViewModel owner) : base(StateName, StateType.Parallel)
        {
            Owner = owner;
            //Visible -> NonInteractive
            STC.IState visibleState = (Owner as IWindowState).VisibleState;
            var transition = new STC.Transition(VisibleState.StateName, NonInteractiveState.StateName);
            transition.AddCondition(new IsNonInteractive(owner));
            visibleState.AddTransition(transition);

            //Interactive -> NonInteractive
            transition = new STC.Transition(InteractiveState.StateName, NonInteractiveState.StateName);
            transition.AddCondition(new IsNonInteractive(owner));
            visibleState.AddTransition(transition);

            //Transition -> NonInteractive
            transition = new STC.Transition(TransitionState.StateName, NonInteractiveState.StateName);
            transition.AddCondition(new IsNoTransition(owner));
            AddTransition(transition);
            //Modal -> NonInteractive
            transition = new STC.Transition(ModalState.StateName, NonInteractiveState.StateName);
            transition.AddCondition(new IsNoModal(owner));
            AddTransition(transition);

        }
    }
}