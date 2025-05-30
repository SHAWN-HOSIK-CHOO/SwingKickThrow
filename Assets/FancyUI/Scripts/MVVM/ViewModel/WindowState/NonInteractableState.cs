using STC;
using static MVVM.ViewModel.Window.State.ModalState;
using static MVVM.ViewModel.Window.State.TransitionState;
using static UnityEngine.UI.GridLayoutGroup;
namespace MVVM.ViewModel.Window.State
{
    public class NonInteractableState : STC.State 
    {
        public const string StateName = "NonInteractable";
        WindowViewModel Owner;

        public NonInteractableState(WindowViewModel owner) : base(StateName, StateType.Parallel)
        {
            Owner = owner;
            //Visible -> Visible.NonInteractive
            STC.IState visibleState = (Owner as IWindowState).VisibleState;
            var barrier = new STC.BoolBarrier(Owner.InteractableTrigger, true, VisibleState.StateName + NonInteractableState.StateName);
            var transition = new STC.Transition(VisibleState.StateName, NonInteractableState.StateName);
            transition.AddCondition(barrier);
            visibleState.AddTransition(transition);

            //Visible.Interactive -> Visible.NonInteractive
            transition = new STC.Transition(InteractableState.StateName, NonInteractableState.StateName);
            transition.AddCondition(barrier);
            visibleState.AddTransition(transition);

            //NonInteractive.Transition -> NonInteractive
            transition = new STC.Transition(TransitionState.StateName, NonInteractableState.StateName);
            barrier = new STC.BoolBarrier(Owner.TransitionTrigger, true, TransitionState.StateName + NonInteractableState.StateName);
            transition.AddCondition(barrier);
            AddTransition(transition);

            //NonInteractive.Modal -> NonInteractive
            transition = new STC.Transition(ModalState.StateName, NonInteractableState.StateName);
            barrier = new STC.BoolBarrier(Owner.ModalTrigger, true, ModalState.StateName + NonInteractableState.StateName);
            transition.AddCondition(barrier);
            AddTransition(transition);

        }
        public override void OnActivate()
        {
            Owner.Binder.InteractableVal.Value = false;
        }

        public override void OnEnter()
        {
            Owner.InteractableTrigger.Value = false;
        }
        public override void OnExit()
        {
            Owner.InteractableTrigger.Value = true;
        }
        public override void OnInActivate()
        {
            if(!Task)
            {
                Owner.Binder.InteractableVal.Value = true;
            }
        }
    }
}