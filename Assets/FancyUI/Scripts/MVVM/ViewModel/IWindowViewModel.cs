using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace MVVM.ViewModel.Window
{
    public interface IWindowViewModel
    {
        STC.IState GetState();
        IWindowViewModel ModalWindow { get; }
        IWindowViewModel ParentWindow { get; }
        List<Window.IWindowViewModel> ChildWindow { get; }


    }
}