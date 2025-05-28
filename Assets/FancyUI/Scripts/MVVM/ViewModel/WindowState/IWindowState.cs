using MVVM.Property;
using UnityEngine;
namespace MVVM.ViewModel.Window
{
    public interface IWindowState
    {
        IWindowViewBinding Binder { get; }
        STC.IState InteractiveState { get; }
        BoolProperty InteractableTrigger { get; }
        STC.IState NonInteractableState { get; }
        STC.IState IDLEState { get; }
        STC.IState TransitionState { get; }
        STC.IState ModalState { get; }
        BoolProperty TransitionTrigger { get; }
        BoolProperty ModalTrigger { get; }
        STC.IState VisibleState { get; }
        STC.IState HiddenState { get; }
        BoolProperty VisibleTrigger { get; }
    }
}