using MVVM.ViewModel.Window.BIND;
using STC;
using static MVVM.ViewModel.Window.State.NonInteractiveState;
namespace MVVM.ViewModel.Window.State
{
    public class VisibleState : STC.State
    {
        public class IsVisible : Condition
        {
            IWindowViewModel Owner;
            public IsVisible(IWindowViewModel owner) : base(nameof(IsVisible), true)
            {
                Owner = owner;
                Owner.Visible.Subscribe(Callback);
            }
            public override bool OnCheck()
            {
                return Owner.Visible.Value;
            }
        }
        public const string StateName = "Visible";
        IWindowViewModel Owner;
        public VisibleState(IWindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState OwnerState = Owner as STC.IState;
            //Onwer -> Visible
            var transition = new STC.Transition(OwnerState.Name, VisibleState.StateName);
            transition.AddCondition(new IsVisible(owner));
            OwnerState.AddTransition(transition);
            
            //Hidden -> Visible 
            transition = new STC.Transition(HiddenState.StateName,VisibleState.StateName);
            transition.AddCondition(new IsVisible(owner));
            OwnerState.AddTransition(transition);
        }

        public override void OnHandle(STC.Event.IStateEvent e)
        {
            if (e is Event.SetModalEvent modalEvent)
            {
                Owner.IsModal.Value = modalEvent.active;
                modalEvent.Consumed.isConsumed = true;
            }
            else if (e is Event.SetInterativeEvent interactEvent)
            {
                Owner.Interactable.Value = interactEvent.interative;
                interactEvent.Consumed.isConsumed = true;

            }
        }

        public override void OnEnter()
        {
            foreach(var w in Owner.ChildWindow)
            {
                var child = w as IViewEventBinding;
                child.ShowWindow();
            }
        }
    }
}