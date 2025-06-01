using MVVM.View.Window;
using UnityEditor;
using UnityEngine;

namespace MVVM.ViewModel.Window
{
    [CustomEditor(typeof(ModalWindow))]
    public class ModalWindowEditor : WindowEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}