using MVVM.ViewModel.Window;
using System;

namespace MVVM.View.Window
{ 
    public class WindowViewBinder
    {
        public IWindowViewBinding ViewBinding;

        Action Visible;
        Action Interactable;
        Action Transition;
        Action Modal;

        public void ForceNotify()
        {
            ViewBinding.Visible.Invoke();
            ViewBinding.Modal.Invoke();
            ViewBinding.Interactable.Invoke();
            ViewBinding.Transition.Invoke();
        }

        public void BindViewModel(string name, bool visible, bool interact, bool modal, bool transition)
        {
            var model = new WindowViewModel(name);
            model.VisibleTrigger.Value = visible;
            model.InteractableTrigger.Value = interact;
            model.ModalTrigger.Value = modal;
            model.TransitionTrigger.Value = transition;

            ViewBinding = model.Binder ;


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
            ViewBinding.Transition.Subscribe(Visible);
        }
    }
}