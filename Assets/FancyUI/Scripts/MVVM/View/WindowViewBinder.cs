using MVVM.ViewModel.Window;
using MVVM.ViewModel.Window.State;
using MVVM.ViewModel.Window.Trigger;
using System;
using Unity.Hierarchy;

namespace MVVM.View.Window
{ 
    public class WindowViewBinder
    {
        public IWindowViewBinding ViewBinding;
        public IWindowTrigger ViewTrigger;
        public WindowHierachyState WindowHierachyState;
        Action Visible;
        Action Interactable;
        Action Transition;
        Action Modal;


        public void BindViewModel(string name, bool visible, bool interact, bool modal, bool transition)
        {
            var model = new WindowViewModel(name + "-ViewModel", visible, interact, modal, transition);
            WindowHierachyState = new WindowHierachyState(model,model.Name + "-Hierarchy");
            ViewTrigger = model;
            ViewBinding = model.Binder;
        }
        public void BindVisibleAction(Action visible)
        {
            Visible = visible;
            ViewBinding.Visible.Subscribe(Visible);
        }
        public void BindModalAction(Action modal)
        {
            Modal = modal;
            ViewBinding.Modal.Subscribe(Modal);
        }
        public void BindInteractiveAction(Action interactable)
        {
            Interactable = interactable;
            ViewBinding.Interactable.Subscribe(Interactable);
        }
        public void BindTransitionAction(Action transition)
        {
            Transition = transition;
            ViewBinding.Transition.Subscribe(transition);
        }
    }
}