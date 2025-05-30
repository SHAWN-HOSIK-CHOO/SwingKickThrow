using STC;
using System.Threading;
using Unity.VisualScripting;
using static UnityEngine.UI.GridLayoutGroup;
namespace MVVM.ViewModel.Window.State
{
    public class VisibleState : STC.State
    {
        public class CanVisible : Condition
        {
            WindowViewModel Owner;
            public CanVisible(WindowViewModel owner) : base(nameof(CanVisible), true)
            {
                Owner = owner;
            }
            public override bool OnCheck()
            {
                var window = Owner.Binder;
                if(window.ParentWindow != null)
                {
                    if(!window.ParentWindow.Visible.Get())
                    {
                        return false;
                    }
                    if (window.ParentWindow.ModalWindow == window && !window.ParentWindow.ModalVal.Value)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public const string StateName = "Visible";
        WindowViewModel Owner;
        public VisibleState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState OwnerState = Owner;
            var cond = new CanVisible(owner);
            var barrier = new STC.BoolBarrier(Owner.VisibleTrigger, false ,OwnerState.Name + VisibleState.StateName);

            //Owner -> Visible
            var transition = new STC.Transition(OwnerState.Name, VisibleState.StateName);
            transition.AddCondition(cond).AddCondition(barrier);
            OwnerState.AddTransition(transition);

            //Owner.Hidden -> Owner.Visible 
            transition = new STC.Transition(HiddenState.StateName,VisibleState.StateName);
            barrier = new STC.BoolBarrier(Owner.VisibleTrigger, false ,HiddenState.StateName + VisibleState.StateName);
            transition.AddCondition(cond).AddCondition(barrier);
            OwnerState.AddTransition(transition);
        }

        public override void OnHandle(STC.Event.IStateEvent e)
        {
            if (e is Event.InteratableEvent interactEvent)
            {
                interactEvent.Consumed.isConsumed = true;
                Owner.InteractableTrigger.Value = interactEvent.interative;
            }
        }

        public override void OnActivate()
        {
            Owner.Binder.VisibleVal.Value = true;
            foreach(var w in Owner.Binder.ChildWindow)
            {
                w.ShowWindow();
            }
        }

        public override void OnEnter()
        {
            Owner.VisibleTrigger.Value = true;
        }


        public override void OnExit()
        {
            Owner.VisibleTrigger.Value = false;
        }
        public override void OnInActivate()
        {
            Owner.Binder.VisibleVal.Value = false;
            foreach (var w in Owner.Binder.ChildWindow)
            {
                w.HideWindow();
            }
        }
    }
}