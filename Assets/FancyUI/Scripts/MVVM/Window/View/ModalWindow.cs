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
        public override void CloseModal()
        {
            __ParentWindow.Binder.ViewBinding.CloseModal();
        }
    }
}