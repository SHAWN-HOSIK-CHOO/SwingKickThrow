using MVVM.ViewModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace MVVM.View.Window
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(EventTrigger))]
    public class ModalWindow : Window
    {
        public Button close; 
        protected override void Start()
        {
            base.Start();
            viewModel.Hide();
            close.onClick.AddListener(OnClose);
        }

        void OnClose()
        {
            if(ParentWindow != null)
            {
                ParentWindow.ModalCloseRequest();
            }
        }
    }
}