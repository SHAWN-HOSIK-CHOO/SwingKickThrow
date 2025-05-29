using STC;
using UnityEngine;
using static MVVM.ViewModel.Window.State.ModalState;
using static MVVM.ViewModel.Window.State.VisibleState;

namespace MVVM.ViewModel.Window.State
{
    public class InteractableState : STC.State 
    {
        public const string StateName = "Interactable";
        WindowViewModel Owner;
        public class CanInteractable : Condition
        {
            WindowViewModel Owner;
            public CanInteractable(WindowViewModel owner) : base(nameof(CanInteractable), true)
            {
                Owner = owner;
            }
            public override bool OnCheck()
            {
                return !Owner.Binder.Transition.Get() && Owner.Binder.Interactable.Get();
            }
        }

        public InteractableState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState visibleState = (Owner as IWindowState).VisibleState;
            //Visible -> Visible.Interactiveon
            var transition = new STC.Transition(VisibleState.StateName, InteractableState.StateName);
            transition.AddCondition(new CanInteractable(owner));
            visibleState.AddTransition(transition);

            //Visible.NonInteractive -> Visible.Interactive
            transition = new STC.Transition(NonInteractableState.StateName,InteractableState.StateName);
            transition.AddCondition(new CanInteractable(owner));
            visibleState.AddTransition(transition);
        }


    }
}