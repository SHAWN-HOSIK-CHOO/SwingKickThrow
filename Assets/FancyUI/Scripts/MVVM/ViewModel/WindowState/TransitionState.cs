using MVVM.ViewModel.Window.BIND;
using STC;
using UnityEngine;
using static MVVM.ViewModel.Window.State.InteractiveState;
namespace MVVM.ViewModel.Window.State
{
    public class TransitionState : STC.State
    {
        public const string StateName = "Transition";
        IWindowViewModel Owner;

        public class IsTransition : Condition
        {
            IWindowViewModel Onwer;
            public IsTransition(IWindowViewModel owner) : base(true)
            {
                Onwer = owner;
                Onwer.IsTransition.Subscribe(Callback);
            }
            public override bool OnCheck()
            {
                return Onwer.IsTransition.Value;
            }
        }
        public class IsNoTransition : Condition
        {
            IWindowViewModel Onwer;
            public IsNoTransition(IWindowViewModel owner) : base(true)
            {
                Onwer = owner;
                Onwer.IsTransition.Subscribe(Callback);
            }
            public override bool OnCheck()
            {
                return !Onwer.IsTransition.Value;
            }
        }

        public TransitionState(IWindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState nonInteractiveState = (Owner as IWindowState).NonInteractiveState;
            //NonInteractive -> Transition
            var transition = new STC.Transition(NonInteractiveState.StateName, TransitionState.StateName);
            transition.AddCondition(new IsTransition(owner));
            nonInteractiveState.AddTransition(transition);
        }
    }
}