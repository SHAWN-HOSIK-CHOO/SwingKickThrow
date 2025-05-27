using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static MVVM.ViewModel.Window.State.TransitionState;
namespace MVVM.ViewModel.Window
{
    public interface IWindowViewModel
    {
        MVVM.Property.Property<bool> Visible { get; }
        Property.Property<bool> Interactable { get; }
        Property.Property<bool> IsModal { get; }
        Property.Property<bool> IsTransition { get; }
        IWindowViewModel ModalWindow { get; }
        IWindowViewModel ParentWindow { get; }
        List<Window.IWindowViewModel> ChildWindow { get; }
        STC.IState GetState();
    }
}