using MVVM.Property;
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
        BoolProperty InteractableStateBarrier;

        public InteractableState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState visibleState = (Owner as IWindowState).VisibleState;

            InteractableStateBarrier = new BoolProperty(nameof(InteractableStateBarrier));
            InteractableStateBarrier.Value = true;
            
            //Visible -> Visible.Interactiveon
            var transition = new STC.Transition(VisibleState.StateName, InteractableState.StateName);
            var barrier = new STC.BoolBarrier(InteractableStateBarrier, true, VisibleState.StateName + InteractableState.StateName);
            transition.Condition.AddCondition(barrier);
            visibleState.AddTransition(transition);

            //Visible.NonInteractive -> Visible.Interactive
            transition = new STC.Transition(NonInteractableState.StateName,InteractableState.StateName);
            barrier = new STC.BoolBarrier(InteractableStateBarrier, true, NonInteractableState.StateName + InteractableState.StateName);
            transition.Condition.AddCondition(barrier);
            visibleState.AddTransition(transition);
        }
        public override void OnCheckTransitionBarrier(string src)
        {
            InteractableStateBarrier.SetSilently(true);

            var window = Owner.Binder;
            bool isParentInteractable = Owner.IsParentInteractable();
            bool hasParent = Owner.HasParentWindow();

            if (hasParent && !isParentInteractable)
            {
                InteractableStateBarrier.SetSilently(true);
            }
            else
            {
                if (Owner.InteractableTrigger.Value)
                {
                    InteractableStateBarrier.SetSilently(false);
                }
            }
        }

        public override void OnActivate()
        {
            Owner.Binder.InteractableVal.Value = true;
        }
        public override void OnEnter()
        {

        }

        public override void OnExit()
        {
            InteractableStateBarrier.SetSilently(true); 
        }

        public override void OnInActivate()
        {
            Owner.Binder.InteractableVal.Value = false;
        }
    }
}