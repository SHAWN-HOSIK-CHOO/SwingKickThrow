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
        public class CanNonInteractable : Condition
        {
            WindowViewModel Owner;
            public CanNonInteractable(WindowViewModel owner) : base(nameof(CanNonInteractable), true)
            {
                Owner = owner;
            }
            public override bool OnCheck()
            {
                return Owner.Binder.Modal.Get() || Owner.Binder.Transition.Get() || !Owner.Binder.Interactable.Get();
            }
        }
        public NonInteractableState(WindowViewModel owner) : base(StateName, StateType.Parallel)
        {
            Owner = owner;
            //Visible -> Visible.NonInteractive
            STC.IState visibleState = (Owner as IWindowState).VisibleState;
            var transition = new STC.Transition(VisibleState.StateName, NonInteractableState.StateName);
            transition.AddCondition(new CanNonInteractable(owner));
            visibleState.AddTransition(transition);

            //Visible.Interactive -> Visible.NonInteractive
            transition = new STC.Transition(InteractableState.StateName, NonInteractableState.StateName);
            transition.AddCondition(new CanNonInteractable(owner));
            visibleState.AddTransition(transition);

            //NonInteractive.Transition -> NonInteractive
            transition = new STC.Transition(TransitionState.StateName, NonInteractableState.StateName);
            transition.AddCondition(new IsNoTransition(owner));
            AddTransition(transition);

            //NonInteractive.Modal -> NonInteractive
            transition = new STC.Transition(ModalState.StateName, NonInteractableState.StateName);
            transition.AddCondition(new IsNoModal(owner));
            AddTransition(transition);

        }
        public override void OnActivate()
        {
            Owner.Binder.InteractableVal.Value = false;
        }
    }
}