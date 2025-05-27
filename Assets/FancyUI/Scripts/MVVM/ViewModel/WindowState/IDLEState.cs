using MVVM.ViewModel.Window.BIND;
using STC;
using UnityEngine;
using static MVVM.ViewModel.Window.State.InteractiveState;
using static UnityEngine.UI.GridLayoutGroup;
namespace MVVM.ViewModel.Window.State
{
    public class IDLEState : STC.State 
    {
        const string StateName = "IDLE";
        IWindowViewModel Owner;
        public class IsIDLE : Condition
        {
            IWindowViewModel Owner;
            public IsIDLE(IWindowViewModel owner) : base( nameof(IsIDLE),true)
            {
                Owner = owner;
                Owner.Interactable.Subscribe(Callback);
            }
            public override bool OnCheck()
            {
                return Owner.Interactable.Value;
            }
        }

        public IDLEState(IWindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState interactiveState = (Owner as IWindowState).InteractiveState;
            //Interactive -> IDLE
            var transition = new STC.Transition(InteractiveState.StateName, IDLEState.StateName);
            transition.AddCondition(new IsIDLE(owner));
            interactiveState.AddTransition(transition);
        }
    }
}