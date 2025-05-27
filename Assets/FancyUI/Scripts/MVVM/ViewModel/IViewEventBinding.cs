using MVVM.ViewModel;
using UnityEngine;
namespace MVVM.ViewModel.Window.BIND
{
    public interface IViewEventBinding
    {
        void SetModal(IWindowViewModel modal);
        void ShowWindow();
        void HideWindow();

        void SetParentWindow(IWindowViewModel parent);
        void AddChildWindow(IWindowViewModel child);
        void OpenModal();
        void CloseModal();
        void OnClick(Vector2 pos);
    }
}