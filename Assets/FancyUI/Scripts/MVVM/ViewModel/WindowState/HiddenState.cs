using MVVM.ViewModel.Window.BIND;
using STC;
using STC.Event;
using UnityEngine;
using static MVVM.ViewModel.Window.State.VisibleState;
using static UnityEngine.UI.GridLayoutGroup;
namespace MVVM.ViewModel.Window.State
{
    public class HiddenState : STC.State
    {
        public class IsHidden : Condition
        {
            IWindowViewModel Owner;
            public IsHidden(IWindowViewModel owner) : base(nameof(IsHidden),true)
            {
                Owner = owner;
                Owner.Visible.Subscribe(Callback);
            }
            public override bool OnCheck()
            {
                return !Owner.Visible.Value;
            }
        }
        public const string StateName = "Hidden";
        IWindowViewModel Owner;
        public HiddenState(IWindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;

            IState OwnerState = Owner as IState;
            //Onwer -> Hidden
            var transition = new STC.Transition(OwnerState.Name, HiddenState.StateName);
            transition.AddCondition(new IsHidden(owner));
            OwnerState.AddTransition(transition);

            //Visible -> Hidden
            transition = new STC.Transition(VisibleState.StateName, HiddenState.StateName);
            transition.AddCondition(new IsHidden(owner));
            OwnerState.AddTransition(transition);
        }

        public override void OnEnter()
        {
            foreach (var w in Owner.ChildWindow)
            {
                var child = w as IViewEventBinding;
                child.HideWindow();
            }
        }

    }
}