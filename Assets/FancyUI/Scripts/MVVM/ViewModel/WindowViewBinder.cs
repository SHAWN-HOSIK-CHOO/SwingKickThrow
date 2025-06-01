using MVVM.Property;
using MVVM.ViewModel.Window.Event;
using MVVM.ViewModel.Window.State;
using STC;
using UnityEngine;
using static STC.StateChart;
namespace MVVM.ViewModel.Window
{ 
    public class WindowViewBinder : IWindowViewBinding
    {
        public class WindowHierachyProperty : Property<WindowHierachyState>
        {

        }

        IPropertySetter<WindowHierachyState> IWindowViewBinding.WindowHierachyVal => WindowHierachy;

        IPropertySetter<bool> IWindowViewBinding.VisibleVal => Visible;

        IPropertySetter<bool> IWindowViewBinding.InteractableVal => Interactable;

        IPropertySetter<bool> IWindowViewBinding.ModalVal => Modal;

        IPropertySetter<bool> IWindowViewBinding.TransitionVal => Transition;

        IPropertyNofier<WindowHierachyState> IWindowViewBinding.WindowHierachy => WindowHierachy;

        IPropertyNofier<bool> IWindowViewBinding.Visible => Visible;

        IPropertyNofier<bool> IWindowViewBinding.Interactable => Interactable;

        IPropertyNofier<bool> IWindowViewBinding.Modal => Modal;

        IPropertyNofier<bool> IWindowViewBinding.Transition => Transition;

        public WindowHierachyProperty WindowHierachy { get; protected set; }
        public BoolProperty Visible { get; protected set; }
        public BoolProperty Interactable { get; protected set; }
        public BoolProperty Modal { get; protected set; }
        public BoolProperty Transition { get; protected set; }

        IState OwnerWindow;

        public WindowViewBinder(IState owner)
        {
            OwnerWindow = owner;
            WindowHierachy = new WindowHierachyProperty();
            Visible = new BoolProperty();
            Modal = new BoolProperty();
            Transition = new BoolProperty();
            Interactable = new BoolProperty();
        }

        void IWindowViewBinding.BindModalWindow(IWindowViewBinding modal)
        {
            WindowHierachy.Value.Handle(new ModalWindowEvent(modal, false, true));
        }

        void IWindowViewBinding.ShowWindow()
        {
            OwnerWindow.Handle(new VisibleWindowEvent(true, false, true));
        }
        void IWindowViewBinding.HideWindow()
        {
            OwnerWindow.Handle(new VisibleWindowEvent(false, false, true));
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
            OwnerWindow.Handle(new InteratableEvent(true, false, true));
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
            OwnerWindow.Handle(new TransitionEvent(false, false, true));
        }

        void IWindowViewBinding.SetParentWindow(IWindowViewBinding parent)
        {
            StateChart.Instance.PublishEvent(new SetParentEvent()
            {
                state = WindowHierachy.Get(),
                parent = parent.WindowHierachy.Get()
            });
        }
        void IWindowViewBinding.RemoveChildWindow(IWindowViewBinding child)
        {
            StateChart.Instance.PublishEvent(new RemoveChildEvent()
            {
                child = child.WindowHierachy.Get()
            });
        }


        void IWindowViewBinding.AddChildWindow(IWindowViewBinding child)
        {
            StateChart.Instance.PublishEvent(new AddChildEvent()
            {
                state = WindowHierachy.Get(),
                child = child.WindowHierachy.Get()
            });
        }

        public void OnClick(Vector2 pos)
        {
            Debug.Log($"Click: {pos}"+ OwnerWindow.Name);
        }
    }
}