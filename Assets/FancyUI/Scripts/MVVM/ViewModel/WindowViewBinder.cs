using MVVM.Property;
using MVVM.View.Window;
using MVVM.ViewModel.Window.Event;
using STC;
using System.Collections.Generic;
using UnityEngine;
namespace MVVM.ViewModel.Window
{ 
    public class WindowViewBinder : IWindowViewBinding
    {
        public Window.IWindowViewBinding ModalWindow { get; private set; }
        public Window.IWindowViewBinding ParentWindow { get; private set; }
        public List<Window.IWindowViewBinding> ChildWindow { get; private set; }

        IPropertySetter<bool> IWindowViewBinding.VisibleVal => Visible;

        IPropertySetter<bool> IWindowViewBinding.InteractableVal => Interactable;

        IPropertySetter<bool> IWindowViewBinding.ModalVal => IsModal;

        IPropertySetter<bool> IWindowViewBinding.TransitionVal => IsTransition;

        IPropertyNofier<bool> IWindowViewBinding.Visible => Visible;

        IPropertyNofier<bool> IWindowViewBinding.Interactable => Interactable;

        IPropertyNofier<bool> IWindowViewBinding.Modal => IsModal;

        IPropertyNofier<bool> IWindowViewBinding.Transition => IsTransition;

        public BoolProperty Visible { get; protected set; }
        public BoolProperty Interactable { get; protected set; }
        public BoolProperty IsModal { get; protected set; }
        public BoolProperty IsTransition { get; protected set; }

        IState OwnerWindow;

        public WindowViewBinder(IState owner)
        {
            OwnerWindow = owner;
            Visible = new BoolProperty();
            IsModal = new BoolProperty();
            IsTransition = new BoolProperty();
            Interactable = new BoolProperty();

            ChildWindow = new List<IWindowViewBinding>();
        }

        void IWindowViewBinding.BindModalWindow(IWindowViewBinding modal)
        {
            ModalWindow = modal;
        }
        void IWindowViewBinding.ShowWindow(bool downStream)
        {
            OwnerWindow.Handle(new SetVisibleWindowEvent(true, downStream));
        }
        void IWindowViewBinding.HideWindow(bool downStream)
        {
            OwnerWindow.Handle(new SetVisibleWindowEvent(false, downStream));
        }
        void IWindowViewBinding.OpenModal(bool downStream)
        {
            OwnerWindow.Handle(new ModalEvent(true, downStream));
        }
        void IWindowViewBinding.CloseModal(bool downStream)
        {
            OwnerWindow.Handle(new ModalEvent(false, downStream));
        }
        void IWindowViewBinding.InteractableWindow(bool downStream)
        {
            OwnerWindow.Handle(new InteratableEvent(true, downStream));
        }
        void IWindowViewBinding.NonInteractableWindow(bool downStream)
        {
            OwnerWindow.Handle(new InteratableEvent(false, downStream));
        }
        public void OnClick(Vector2 pos)
        {

        }
        void IWindowViewBinding.SetParentWindow(IWindowViewBinding parent)
        {
            if (ParentWindow == parent) return;
            if (ParentWindow != null)
            {
                var w = ParentWindow ;
                w.RemoveChildWindow(this);
            }
            ParentWindow = parent;
            if (ParentWindow != null)
            {
                var w = ParentWindow;
                w.AddChildWindow(this);
            }
        }
        void IWindowViewBinding.RemoveChildWindow(IWindowViewBinding child)
        {
            if (!ChildWindow.Contains(child )) return;
            ChildWindow.Remove(child );
            var w = child as IWindowViewBinding;
            w.SetParentWindow(null);
        }
        void IWindowViewBinding.AddChildWindow(IWindowViewBinding child)
        {
            if (ChildWindow.Contains(child )) return;
            ChildWindow.Add(child );
            var w = child as IWindowViewBinding;
            w.SetParentWindow(this);
        }
    }
}