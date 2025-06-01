using MVVM.Property;
using STC;
using System.Threading;
using static MVVM.ViewModel.Window.State.IDLEState;
namespace MVVM.ViewModel.Window.State
{
    public class ModalState : STC.State
    {
        public const string StateName = "Modal";
        WindowViewModel Owner;

        BoolProperty ModalStateBarrier;

        public ModalState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState nonInteractiveState = (Owner as IWindowState).NonInteractableState;
            ModalStateBarrier = new BoolProperty();
            ModalStateBarrier.Value = true;

            //NonInteractive ->NonInteractive.Modal
            var transition = new STC.Transition(NonInteractableState.StateName, ModalState.StateName);
            var barrier = new STC.BoolBarrier(ModalStateBarrier, true, NonInteractableState.StateName + ModalState.StateName);
            transition.Condition.AddCondition(barrier);
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

        public override void OnCheckTransitionBarrier(string src)
        {
            ModalStateBarrier.SetSilently(true);

            if(Owner.ModalTrigger.Value)
            {
                ModalStateBarrier.SetSilently(false);
            }
        }

        public override void OnActivate()
        {
            Owner.Binder.ModalVal.Value = true;

            if (Owner.Binder.WindowHierachy.Get() is WindowHierachyState Hierarchy)
            {
                if (Hierarchy.ModalHierachy != null)
                {
                    Hierarchy.ModalHierachy.ViewModel.Binder.ShowWindow();
                }
            }
        }
        public override void OnEnter()
        {

        }
        public override void OnExit()
        {
            ModalStateBarrier.SetSilently(true);
        }

        public override void OnInActivate()
        {
            Owner.Binder.ModalVal.Value = false;
            if (Owner.Binder.WindowHierachy.Get() is WindowHierachyState Hierarchy)
            {
                if(Hierarchy.ModalHierachy != null)
                {
                    Hierarchy.ModalHierachy.ViewModel.Binder.HideWindow();
                }

            }
        }
    }
}