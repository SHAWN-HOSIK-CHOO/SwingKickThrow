using MVVM.Property;
using STC;
using System.Collections.Generic;
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
        BoolProperty TransitionStateBarrier;

        public TransitionState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState nonInteractiveState = (Owner as IWindowState).NonInteractableState;
            TransitionStateBarrier = new BoolProperty();
            TransitionStateBarrier.Value = true;


            //NonInteractable -> NonInteractableState.Transition
            var transition = new STC.Transition(NonInteractableState.StateName, TransitionState.StateName);
            var barrier = new STC.BoolBarrier(TransitionStateBarrier, true, NonInteractableState.StateName + TransitionState.StateName);
            transition.Condition.AddCondition(barrier);
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

        }
        public override void OnCheckTransitionBarrier(string src)
        {
            TransitionStateBarrier.SetSilently(true);
            if (Owner.TransitionTrigger.Value)
            {
                TransitionStateBarrier.SetSilently(false);
            }
        }
        public override void OnExit()
        {
            TransitionStateBarrier.SetSilently (true);
        }

        public override void OnInActivate()
        {
            Owner.Binder.TransitionVal.Value = false;
        }
    }
}