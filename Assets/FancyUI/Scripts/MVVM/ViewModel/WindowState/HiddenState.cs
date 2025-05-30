using STC;
using System.Threading;
using static MVVM.ViewModel.Window.State.VisibleState;
using static UnityEngine.UI.GridLayoutGroup;
namespace MVVM.ViewModel.Window.State
{
    public class HiddenState : STC.State
    {
        public class CanHidden : Condition
        {
            WindowViewModel Owner;
            public CanHidden(WindowViewModel owner) : base(nameof(CanHidden),true)
            {
                Owner = owner;
            }
            public override bool OnCheck()
            {
                var window = Owner.Binder;
                if (window.ParentWindow != null)
                {
                    if (!window.ParentWindow.Visible.Get())
                    {
                        return true;
                    }
                    if (window.ParentWindow.ModalWindow == window && window.ParentWindow.ModalVal.Value)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public const string StateName = "Hidden";
        WindowViewModel Owner;
        public HiddenState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            IState OwnerState = Owner as IState;
            var cond = new CanHidden(owner);


            //Owner -> Hidden
            var transition = new STC.Transition(OwnerState.Name, HiddenState.StateName);
            var barrier = new STC.BoolBarrier(Owner.VisibleTrigger, true, OwnerState.Name + HiddenState.StateName);
            transition.AddCondition(cond).AddCondition(barrier);
            OwnerState.AddTransition(transition);


            //Visible -> Hidden
            transition = new STC.Transition(VisibleState.StateName, HiddenState.StateName);
            barrier = new STC.BoolBarrier(Owner.VisibleTrigger, true, VisibleState.StateName + HiddenState.StateName);
            transition.AddCondition(cond).AddCondition(barrier);
            OwnerState.AddTransition(transition);
        }

        public override void OnActivate()
        {
            Owner.Binder.VisibleVal.Value = false;
            foreach (var w in Owner.Binder.ChildWindow)
            {
                w.HideWindow();
            }
        }

        public override void OnEnter()
        {
            Owner.VisibleTrigger.Value = false;
        }

        public override void OnExit()
        {
            Owner.VisibleTrigger.Value = true;
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