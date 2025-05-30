using STC;
using System.Threading;
using static MVVM.ViewModel.Window.State.IDLEState;
namespace MVVM.ViewModel.Window.State
{
    public class ModalState : STC.State
    {
        public const string StateName = "Modal";
        WindowViewModel Owner;
       
        public ModalState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState nonInteractiveState = (Owner as IWindowState).NonInteractableState;
            //NonInteractive ->NonInteractive.Modal
            var transition = new STC.Transition(NonInteractableState.StateName, ModalState.StateName);
            var barrier = new STC.BoolBarrier(Owner.ModalTrigger, false, NonInteractableState.StateName + ModalState.StateName);
            transition.AddCondition(barrier);
            nonInteractiveState.AddTransition(transition);
        }

        public override void OnHandle(STC.Event.IStateEvent e)
        {
            if (e is Event.ModalEvent modalEvent)
            {
                modalEvent.Consumed.isConsumed = true;
                Owner.ModalTrigger.Value = modalEvent.Modal;
            }
        }
        public override void OnActivate()
        {
            Owner.Binder.ModalVal.Value = true;
            if (Owner.Binder.ModalWindow != null)
            {
                Owner.Binder.ModalWindow.ShowWindow();
            }
        }
        public override void OnEnter()
        {
            Owner.ModalTrigger.Value = true;
        }
        public override void OnExit()
        {
            Owner.ModalTrigger.Value = false;
        }

        public override void OnInActivate()
        {
            Owner.Binder.ModalVal.Value = false;
            if (Owner.Binder.ModalWindow != null)
            {
                Owner.Binder.ModalWindow.HideWindow();
            }
        }
    }
}