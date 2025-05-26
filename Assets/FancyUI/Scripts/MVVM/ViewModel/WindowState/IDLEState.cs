using UnityEngine;
namespace MVVM.ViewModel.Window.State
{
    public class IDLEState : STC.State 
    {
        const string StateName = "IDLE";
        public IDLEState(IWindowViewModel owner) : base(StateName)
        {

        }
    }
}