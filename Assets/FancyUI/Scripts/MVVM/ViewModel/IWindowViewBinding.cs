using MVVM.Property;
using System.Collections.Generic;
using UnityEngine;
namespace MVVM.ViewModel.Window
{
    public interface IWindowViewBinding
    {
        Window.IWindowViewBinding ModalWindow { get; }
        Window.IWindowViewBinding ParentWindow { get;}
        List<Window.IWindowViewBinding> ChildWindow { get; }

        IPropertySetter<bool> VisibleVal { get; }
        IPropertySetter<bool> InteractableVal { get; }
        IPropertySetter<bool> ModalVal { get; }
        IPropertySetter<bool> TransitionVal { get; }

        IPropertyNofier<bool> Visible { get; }
        IPropertyNofier<bool> Interactable { get; }
        IPropertyNofier<bool> Modal { get; }
        IPropertyNofier<bool> Transition { get; }

        void ShowWindow(bool downStream);
        void HideWindow(bool downStream);
        void InteractableWindow(bool downStream);
        void NonInteractableWindow(bool downStream);
        void OpenModal(bool downStream);
        void CloseModal(bool downStream);
        void BindModalWindow(IWindowViewBinding modal);
        void OnClick(Vector2 pos);

        void SetParentWindow(IWindowViewBinding parent);
        void AddChildWindow(IWindowViewBinding child);
        void RemoveChildWindow(IWindowViewBinding child);
    }
}