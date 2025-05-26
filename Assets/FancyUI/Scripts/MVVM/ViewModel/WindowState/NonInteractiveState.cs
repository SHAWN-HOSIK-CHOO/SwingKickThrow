using UnityEngine;
namespace MVVM.ViewModel.Window.State
{
    public class NonInteractiveState : STC.State 
    {
        public const string StateName = "NonInteractive";
        public NonInteractiveState(IWindowViewModel owner) : base(StateName)
        {

        }  
    }
}