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

        IPropertySetter<bool> IWindowViewBinding.ModalVal => Modal;

        IPropertySetter<bool> IWindowViewBinding.TransitionVal => Transition;

        IPropertyNofier<bool> IWindowViewBinding.Visible => Visible;

        IPropertyNofier<bool> IWindowViewBinding.Interactable => Interactable;

        IPropertyNofier<bool> IWindowViewBinding.Modal => Modal;

        IPropertyNofier<bool> IWindowViewBinding.Transition => Transition;

        public BoolProperty Visible { get; protected set; }
        public BoolProperty Interactable { get; protected set; }
        public BoolProperty Modal { get; protected set; }
        public BoolProperty Transition { get; protected set; }

        IState OwnerWindow;

        public WindowViewBinder(IState owner, bool visible, bool interact, bool modal, bool transition)
        {
            OwnerWindow = owner;
            Visible = new BoolProperty();
            Visible.Value = visible;
            Modal = new BoolProperty();
            Modal.Value = modal;
            Transition = new BoolProperty();
            Transition.Value = transition;
            Interactable = new BoolProperty();
            Interactable.Value = interact;
            ChildWindow = new List<IWindowViewBinding>();
        }

        void IWindowViewBinding.BindModalWindow(IWindowViewBinding modal)
        {
            ModalWindow = modal;
        }
        void IWindowViewBinding.ShowWindow()
        {
            OwnerWindow.Handle(new VisibleWindowEvent(true, true , true));
        }
        void IWindowViewBinding.HideWindow()
        {
            OwnerWindow.Handle(new VisibleWindowEvent(false, true, true));
        }
        void IWindowViewBinding.OpenModal()
        {
            OwnerWindow.Handle(new ModalEvent(true, false, true));
        }
        void IWindowViewBinding.CloseModal()
        {
            OwnerWindow.Handle(new ModalEvent(false, true, true));
        }
        void IWindowViewBinding.InteractableWindow()
        {
            OwnerWindow.Handle(new InteratableEvent(true,false, true));
        }
        void IWindowViewBinding.NonInteractableWindow()
        {
            OwnerWindow.Handle(new InteratableEvent(false, false, true));
        }

        public void TransitionStart()
        {
            OwnerWindow.Handle(new TransitionEvent(true, false, true));
        }

        public void TransitionEnd()
        {
            OwnerWindow.Handle(new TransitionEvent(false, true, true));
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

        public void OnClick(Vector2 pos)
        {

        }
    }
}