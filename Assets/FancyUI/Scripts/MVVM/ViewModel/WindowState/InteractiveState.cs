using MVVM.ViewModel.Window.BIND;
using STC;
using UnityEngine;
using static MVVM.ViewModel.Window.State.ModalState;
using static MVVM.ViewModel.Window.State.VisibleState;
using static UnityEngine.UI.GridLayoutGroup;

namespace MVVM.ViewModel.Window.State
{
    public class InteractiveState : STC.State 
    {
        public const string StateName = "Interactive";
        IWindowViewModel Owner;
        public class IsInteractive : Condition
        {
            IWindowViewModel Owner;
            public IsInteractive(IWindowViewModel owner) : base(nameof(IsInteractive), true)
            {
                Owner = owner;
                Owner.Visible.Subscribe(Callback);
            }
            public override bool OnCheck()
            {
                return Owner.Interactable.Value;
            }
        }

        public InteractiveState(IWindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState visibleState = (Owner as IWindowState).VisibleState;

            //Visible -> Interactive
            var transition = new STC.Transition(VisibleState.StateName, InteractiveState.StateName);
            transition.AddCondition(new IsInteractive(owner));
            visibleState.AddTransition(transition);
            //NonInteractive -> Interactive
            transition = new STC.Transition(NonInteractiveState.StateName,InteractiveState.StateName);
            transition.AddCondition(new IsInteractive(owner));
            visibleState.AddTransition(transition);
        }
    }
}