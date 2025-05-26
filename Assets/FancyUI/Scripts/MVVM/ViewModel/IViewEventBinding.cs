using MVVM.ViewModel;
using UnityEngine;
namespace MVVM.ViewModel.Window.BIND
{
    public interface IViewEventBinding
    {
        public STC.IState InteractiveState { get; }
        public STC.IState NonInteractiveState { get; }
        public STC.IState IDLEState { get;}
        public STC.IState TransitionState { get; }
        public STC.IState ModalState { get;  }
        public STC.IState VisibleState { get;}
        public STC.IState HiddenState { get; }
        void SetModal(IWindowViewModel modal);
        void SetParentWindow(IWindowViewModel parent);
        void AddChildWindow(IWindowViewModel child);
        void OpenModal();
        void CloseModal();
        void Visible();
        void Hide();
        void OnClick(Vector2 pos);


    }
}