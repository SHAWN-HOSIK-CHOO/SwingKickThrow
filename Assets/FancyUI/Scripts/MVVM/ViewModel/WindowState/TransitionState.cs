using STC;
using UnityEngine;
using static MVVM.ViewModel.Window.State.InteractableState;
namespace MVVM.ViewModel.Window.State
{
    public class TransitionState : STC.State
    {
        public const string StateName = "Transition";
        WindowViewModel Owner;

        public class IsTransition : Condition
        {
            WindowViewModel Owner;
            public IsTransition(WindowViewModel owner) : base(nameof(TransitionState), true)
            {
                Owner = owner;
            }
            public override bool OnCheck()
            {
                return Owner.Binder.Transition.Get();
            }
        }
        public class IsNoTransition : Condition
        {
            WindowViewModel Owner;
            public IsNoTransition(WindowViewModel owner) : base(nameof(IsNoTransition),true)
            {
                Owner = owner;
            }
            public override bool OnCheck()
            {
                return !Owner.Binder.Transition.Get();
            }
        }

        public TransitionState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState nonInteractiveState = (Owner as IWindowState).NonInteractableState;
            //NonInteractable -> NonInteractableState.Transition
            var transition = new STC.Transition(NonInteractableState.StateName, TransitionState.StateName);
            transition.AddCondition(new IsTransition(owner));
            nonInteractiveState.AddTransition(transition);

            //NonInteractableState.Transition -> NonInteractable
            transition = new STC.Transition(TransitionState.StateName,NonInteractableState.StateName);
            transition.AddCondition(new IsNoTransition(owner));
            nonInteractiveState.AddTransition(transition);
        }

        public override void OnHandle(STC.Event.IStateEvent e)
        {
            if (e is Event.TransitionEvent transitionEvent)
            {
                transitionEvent.Consumed.isConsumed = true;
                Owner.Binder.TransitionVal.Value = transitionEvent.State;
            }
        }


        public override void OnActivate()
        {
        }
        public override void OnInActivate()
        {

        }
    }
}