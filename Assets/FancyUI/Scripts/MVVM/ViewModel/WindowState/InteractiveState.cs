using UnityEngine;

namespace MVVM.ViewModel.Window.State
{
    public class InteractiveState : STC.State 
    {
        public const string StateName = "Interactive";
        public InteractiveState(IWindowViewModel owner) : base(StateName)
        {

        }
    }
}