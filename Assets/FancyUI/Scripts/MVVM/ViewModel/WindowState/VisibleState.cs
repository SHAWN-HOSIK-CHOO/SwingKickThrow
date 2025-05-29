using STC;
using Unity.VisualScripting;
namespace MVVM.ViewModel.Window.State
{
    public class VisibleState : STC.State
    {
        public class CanVisible : Condition
        {
            WindowViewModel Owner;
            public CanVisible(WindowViewModel owner) : base(nameof(CanVisible), true)
            {
                Owner = owner;
            }
            public override bool OnCheck()
            {
                var window = Owner.Binder;
                if(window.ParentWindow != null && window.ParentWindow.ModalWindow == window)
                {
                    return (Owner.Binder.Visible.Get()) && (window.ParentWindow.ModalVal.Value);
                }
                return (Owner.Binder.Visible.Get());
            }
        }
        public const string StateName = "Visible";
        WindowViewModel Owner;
        public VisibleState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState OwnerState = Owner as STC.IState;
            //Owner -> Visible
            var transition = new STC.Transition(OwnerState.Name, VisibleState.StateName);
            transition.AddCondition(new CanVisible(owner));
            OwnerState.AddTransition(transition);

            //Owner.Hidden -> Owner.Visible 
            transition = new STC.Transition(HiddenState.StateName,VisibleState.StateName);
            transition.AddCondition(new CanVisible(owner));
            OwnerState.AddTransition(transition);
        }

        public override void OnHandle(STC.Event.IStateEvent e)
        {
            if (e is Event.InteratableEvent interactEvent)
            {
                interactEvent.Consumed.isConsumed = true;
                Owner.Binder.InteractableVal.Value = interactEvent.interative;
            }
        }


        public override void OnActivate()
        {
            foreach(var w in Owner.Binder.ChildWindow)
            {
                w.ShowWindow();
            }
        }
    }
}