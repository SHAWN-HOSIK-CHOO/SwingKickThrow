using STC;
using static MVVM.ViewModel.Window.State.VisibleState;
namespace MVVM.ViewModel.Window.State
{
    public class HiddenState : STC.State
    {
        public class CanHidden : Condition
        {
            WindowViewModel Owner;
            public CanHidden(WindowViewModel owner) : base(nameof(CanHidden),true)
            {
                Owner = owner;
            }
            public override bool OnCheck()
            {
                var window = Owner.Binder;
                if (window.ParentWindow != null && window.ParentWindow.ModalWindow == window)
                {
                    return (!Owner.Binder.Visible.Get()) && (!window.ParentWindow.Modal.Get());
                }
                return !Owner.Binder.Visible.Get();
            }
        }
        public const string StateName = "Hidden";
        WindowViewModel Owner;
        public HiddenState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            IState OwnerState = Owner as IState;
            //Owner -> Hidden
            var transition = new STC.Transition(OwnerState.Name, HiddenState.StateName);
            transition.AddCondition(new CanHidden(owner));
            OwnerState.AddTransition(transition);

            //Visible -> Hidden
            transition = new STC.Transition(VisibleState.StateName, HiddenState.StateName);
            transition.AddCondition(new CanHidden(owner));
            OwnerState.AddTransition(transition);
        }

        public override void OnActivate()
        {
            foreach (var w in Owner.Binder.ChildWindow)
            {
                w.HideWindow();
            }
        }
    }
}