using STC;
using UnityEngine;
using static MVVM.ViewModel.Window.State.InteractableState;
using static MVVM.ViewModel.Window.State.ModalState;
using static UnityEngine.UI.GridLayoutGroup;
namespace MVVM.ViewModel.Window.State
{
    public class TransitionState : STC.State
    {
        public const string StateName = "Transition";
        WindowViewModel Owner;
        public TransitionState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState nonInteractiveState = (Owner as IWindowState).NonInteractableState;
            var barrier = new STC.BoolBarrier(Owner.TransitionTrigger, false, NonInteractableState.StateName + TransitionState.StateName);
            
            //NonInteractable -> NonInteractableState.Transition
            var transition = new STC.Transition(NonInteractableState.StateName, TransitionState.StateName);
            transition.AddCondition(barrier);
            nonInteractiveState.AddTransition(transition);
        }

        public override void OnHandle(STC.Event.IStateEvent e)
        {
            if (e is Event.TransitionEvent transitionEvent)
            {
                transitionEvent.Consumed.isConsumed = true;
                Owner.TransitionTrigger.Value = transitionEvent.State;
            }
        }

        public override void OnActivate()
        {
            Owner.Binder.TransitionVal.Value = true;
        }
        public override void OnEnter()
        {
            Owner.TransitionTrigger.Value = true;
        }

        public override void OnExit()
        {
            Owner.TransitionTrigger.Value = false;
        }

        public override void OnInActivate()
        {
            Owner.Binder.TransitionVal.Value = false;
        }
    }
}