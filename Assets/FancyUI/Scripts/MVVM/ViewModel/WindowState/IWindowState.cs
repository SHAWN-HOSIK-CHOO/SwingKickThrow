using MVVM.Property;
using UnityEngine;
namespace MVVM.ViewModel.Window
{
    public interface IWindowState
    {
        IWindowViewBinding Binder { get; }
        STC.IState InteractiveState { get; }
        STC.IState NonInteractableState { get; }
        STC.IState IDLEState { get; }
        STC.IState TransitionState { get; }
        STC.IState ModalState { get; }
        STC.IState VisibleState { get; }
        STC.IState HiddenState { get; }
    }
}