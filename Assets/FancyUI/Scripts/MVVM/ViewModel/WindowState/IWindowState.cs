using UnityEngine;
namespace MVVM.ViewModel.Window
{
    public interface IWindowState
    {
        public STC.IState InteractiveState { get; }
        public STC.IState NonInteractiveState { get; }
        public STC.IState IDLEState { get; }
        public STC.IState TransitionState { get; }
        public STC.IState ModalState { get; }
        public STC.IState VisibleState { get; }
        public STC.IState HiddenState { get; }
    }
}