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

        public InteractableState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState visibleState = (Owner as IWindowState).VisibleState;
            var barrier = new STC.BoolBarrier(Owner.InteractableTrigger, false, VisibleState.StateName + InteractableState.StateName);
            
            //Visible -> Visible.Interactiveon
            var transition = new STC.Transition(VisibleState.StateName, InteractableState.StateName);
            transition.AddCondition(barrier);
            visibleState.AddTransition(transition);

            //Visible.NonInteractive -> Visible.Interactive
            transition = new STC.Transition(NonInteractableState.StateName,InteractableState.StateName);
            transition.AddCondition(barrier);
            visibleState.AddTransition(transition);
        }


        public override void OnActivate()
        {
            Owner.Binder.InteractableVal.Value = true;
        }
        public override void OnEnter()
        {
            Owner.InteractableTrigger.Value = true;
        }

        public override void OnExit()
        {
            Owner.InteractableTrigger.Value = false; 
        }

        public override void OnInActivate()
        {
            Owner.Binder.InteractableVal.Value = false;
        }
    }
}