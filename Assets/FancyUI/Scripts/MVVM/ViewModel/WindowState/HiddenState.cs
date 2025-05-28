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
                Owner.VisibleTrigger.Subscribe(CheckDirty);
            }
            public override bool OnCheck()
            {
                return !Owner.VisibleTrigger.Value;
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
            Owner.Binder.VisibleVal.Value = false;
        }
    }
}