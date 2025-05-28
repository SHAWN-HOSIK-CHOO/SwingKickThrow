using STC;
using static MVVM.ViewModel.Window.State.IDLEState;
namespace MVVM.ViewModel.Window.State
{
    public class ModalState : STC.State
    {
        public const string StateName = "Modal";
        WindowViewModel Owner;
        public class IsModal : Condition
        {
            WindowViewModel Owner;
            public IsModal(WindowViewModel owner) : base(nameof(IsModal),true)
            {
                Owner = owner;
                Owner.ModalTrigger.Subscribe(CheckDirty);
            }
            public override bool OnCheck()
            {
                return Owner.ModalTrigger.Value;
            }
        }
        public class IsNoModal : Condition
        {
            WindowViewModel Owner;
            public IsNoModal(WindowViewModel owner) : base(nameof(IsNoModal), true)
            {
                Owner = owner;
                Owner.ModalTrigger.Subscribe(CheckDirty);
            }
            public override bool OnCheck()
            {
                return !Owner.ModalTrigger.Value;
            }
        }
        public ModalState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState nonInteractiveState = (Owner as IWindowState).NonInteractableState;
            //NonInteractive -> Modal
            var transition = new STC.Transition(NonInteractableState.StateName, ModalState.StateName);
            transition.AddCondition(new IsModal(owner));
            nonInteractiveState.AddTransition(transition);
        }

        public override void OnHandle(STC.Event.IStateEvent e)
        {
            if (e is Event.ModalEvent modalEvent)
            {
                modalEvent.Consumed.isConsumed = true;
                Owner.ModalTrigger.Value = modalEvent.active;
            }
        }
        public override void OnActivate()
        {
            var view = Owner.Binder as IWindowViewBinding;
            view.ModalVal.Value = true;
        }
        public override void OnInActivate()
        {
            var view = Owner.Binder as IWindowViewBinding;
            view.ModalVal.Value = false;
        }
    }
}