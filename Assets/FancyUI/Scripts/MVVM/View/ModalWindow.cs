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
            if (ParentWindow != null)
            {
                ParentWindow.Binder.ViewEventBinding.SetModal(Binder.ViewModel);
                if (gameObject.activeSelf)
                {
                    ParentWindow.Binder.ViewEventBinding.OpenModal();
                }
                else
                {
                    ParentWindow.Binder.ViewEventBinding.CloseModal();
                }
            }
            else
            {
                Binder.ViewEventBinding.CloseModal();
            }

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