using STC;
using UnityEngine;
using static MVVM.ViewModel.Window.State.InteractiveState;
namespace MVVM.ViewModel.Window.State
{
    public class IDLEState : STC.State 
    {
        const string StateName = "IDLE";
        WindowViewModel Owner;
        public IDLEState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState interactiveState = (Owner as IWindowState).InteractiveState;
            //Interactive -> IDLE
            var transition = new STC.Transition(InteractiveState.StateName, IDLEState.StateName);
            interactiveState.AddTransition(transition);
        }
    }
}