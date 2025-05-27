using MVVM.ViewModel.Window.BIND;
using STC;
namespace MVVM.ViewModel.Window.State
{
    public class VisibleState : STC.State
    {
        public class IsVisible : Condition
        {
            IWindowViewModel Owner;
            public IsVisible(IWindowViewModel owner) : base(true)
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
                IViewEventBinding modalWindow = Owner.ModalWindow as IViewEventBinding;
                IViewEventBinding ownerWindow = Owner as IViewEventBinding;
                Owner.IsModal.Value = modalEvent.active;
                modalEvent.Consumed.isConsumed = true;
            }
        }
    }
}