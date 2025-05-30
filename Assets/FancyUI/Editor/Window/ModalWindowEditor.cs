using MVVM.View.Window;
using UnityEditor;

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