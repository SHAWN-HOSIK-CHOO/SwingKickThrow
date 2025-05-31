using MVVM.Property;
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
        BoolProperty NonInteractableStateBarrier;
        BoolProperty NonModalStateBarrier;
        BoolProperty NonTransitionStateBarrier;

        public NonInteractableState(WindowViewModel owner) : base(StateName, StateType.Parallel)
        {
            Owner = owner;
            STC.IState visibleState = (Owner as IWindowState).VisibleState;

            NonInteractableStateBarrier = new BoolProperty(nameof(NonInteractableStateBarrier));
            NonInteractableStateBarrier.Value = true;

            NonModalStateBarrier = new BoolProperty(nameof(NonModalStateBarrier));
            NonModalStateBarrier.Value = true;

            NonTransitionStateBarrier = new BoolProperty(nameof(NonTransitionStateBarrier));
            NonTransitionStateBarrier.Value = true;
            //Visible -> Visible.NonInteractive
            var transition = new STC.Transition(VisibleState.StateName, NonInteractableState.StateName);
            var barrier = new STC.BoolBarrier(NonInteractableStateBarrier, true, VisibleState.StateName + NonInteractableState.StateName);
            transition.Condition.AddCondition(barrier);
            visibleState.AddTransition(transition);

            //Visible.Interactive -> Visible.NonInteractive
            transition = new STC.Transition(InteractableState.StateName, NonInteractableState.StateName);
            barrier = new STC.BoolBarrier(NonInteractableStateBarrier, true, InteractableState.StateName + NonInteractableState.StateName);
            transition.Condition.AddCondition(barrier);
            visibleState.AddTransition(transition);

            //NonInteractive.Transition -> NonInteractive
            transition = new STC.Transition(TransitionState.StateName, NonInteractableState.StateName);
            barrier = new STC.BoolBarrier(NonTransitionStateBarrier, true, TransitionState.StateName + NonInteractableState.StateName);
            transition.Condition.AddCondition(barrier);
            AddTransition(transition);

            //NonInteractive.Modal -> NonInteractive
            transition = new STC.Transition(ModalState.StateName, NonInteractableState.StateName);
            barrier = new STC.BoolBarrier(NonModalStateBarrier, true, ModalState.StateName + NonInteractableState.StateName);
            transition.Condition.AddCondition(barrier);
            AddTransition(transition);

        }
        public override void OnActivate()
        {
            Owner.Binder.InteractableVal.Value = false;
        }
        public override void OnBeforeCheckTranstion(STC.ITransition transition)
        {
            NonInteractableStateBarrier.SetSilently(Owner.InteractableTrigger.Value);

            if(!Owner.InteractableTrigger.Value)
            {
                NonModalStateBarrier.SetSilently(Owner.ModalTrigger.Value);
                NonTransitionStateBarrier.SetSilently(Owner.TransitionTrigger.Value);
            }


        }
        public override void OnEnter()
        {

        }
        public override void OnExit()
        {
            NonInteractableStateBarrier.SetSilently(true);
        }
        public override void OnInActivate()
        {

        }
    }
}