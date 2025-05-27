using MVVM.Property;
using MVVM.ViewModel.Window.BIND;
using MVVM.ViewModel.Window;
namespace MVVM.View.Window
{ 
    public class WindowViewBinder
    {
        public IViewEventBinding ViewEventBinding;
        public IWindowViewModel ViewModel;

        IProperty.DelegateSubscribe Visible;
        IProperty.DelegateSubscribe Interactable;
        IProperty.DelegateSubscribe Transition;
        IProperty.DelegateSubscribe Modal;

        public void BindViewModel(string name)
        {
            WindowViewModel model = new WindowViewModel(name);
            ViewModel = model;
            ViewEventBinding = model;
        }
        public void BindVisibleAction(IProperty.DelegateSubscribe visible)
        {
            Visible = visible;
            ViewModel.Visible.Subscribe(Visible);
        }
        public void BindModalAction(IProperty.DelegateSubscribe modal)
        {
            Modal = modal;
            ViewModel.IsModal.Subscribe(Modal);
        }
        public void BindInteractiveAction(IProperty.DelegateSubscribe interactable)
        {
            Interactable = interactable;
            ViewModel.Interactable.Subscribe(Interactable);
        }
        public void BindTransitionAction(IProperty.DelegateSubscribe transition)
        {
            Transition = transition;
            ViewModel.IsTransition.Subscribe(Visible);
        }

    }
}