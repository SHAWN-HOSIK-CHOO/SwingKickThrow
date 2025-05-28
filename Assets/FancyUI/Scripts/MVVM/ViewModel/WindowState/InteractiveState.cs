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
        WindowViewModel Owner;
        public class CanInteractable : Condition
        {
            WindowViewModel Owner;
            public CanInteractable(WindowViewModel owner) : base(nameof(CanInteractable), true)
            {
                Owner = owner;
                Owner.InteractableTrigger.Subscribe(CheckDirty);
                Owner.ModalTrigger.Subscribe(CheckDirty);
                Owner.TransitionTrigger.Subscribe(CheckDirty);
            }
            public override bool OnCheck()
            {
                return !(Owner.ModalTrigger.Value || Owner.TransitionTrigger.Value) && Owner.InteractableTrigger.Value;
            }
        }

        public InteractiveState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState visibleState = (Owner as IWindowState).VisibleState;

            //Visible -> Visible.Interactiveon
            var transition = new STC.Transition(VisibleState.StateName, InteractiveState.StateName);
            transition.AddCondition(new CanInteractable(owner));
            visibleState.AddTransition(transition);

            //Visible.NonInteractive -> Visible.Interactive
            transition = new STC.Transition(NonInteractableState.StateName,InteractiveState.StateName);
            transition.AddCondition(new CanInteractable(owner));
            visibleState.AddTransition(transition);
        }

        public override void OnActivate()
        {
            Owner.Binder.InteractableVal.Value = true;
        }

    }
}