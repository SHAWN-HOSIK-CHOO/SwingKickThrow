using UnityEngine;
namespace MVVM.ViewModel.Window.State
{
    public class TransitionState : STC.State
    {
        public const string StateName = "Transition";
        IWindowViewModel Owner;
        public TransitionState(IWindowViewModel owner) : base(StateName)
        {
            Owner = owner;
        }
    }
}