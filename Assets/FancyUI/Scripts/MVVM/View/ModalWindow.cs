using MVVM.Property;
using MVVM.ViewModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace MVVM.View.Window
{
    public class ModalWindow : Window
    {
        public Button closeButton;
        protected override void Start()
        {
            base.Start();
            closeButton.onClick.AddListener(OnClose);
        }

        void OnClose()
        {
            Binder.ViewBinding.HideWindow();
        }
    }
}