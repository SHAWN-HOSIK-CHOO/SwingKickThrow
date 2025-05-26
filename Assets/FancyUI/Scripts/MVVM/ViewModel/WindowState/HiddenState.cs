using UnityEngine;
namespace MVVM.ViewModel.Window.State
{
    public class HiddenState : STC.State
    {
        public const string StateName = "Hidden";
        IWindowViewModel Owner;
        public HiddenState(IWindowViewModel owner) : base(StateName)
        {
            Owner = owner;
        }
    }
}