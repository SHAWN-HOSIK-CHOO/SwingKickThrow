using FancyUI.MVVM.View.Transition;
using FancyUI.Transition;
using UnityEditor;
using UnityEngine;

namespace FancyUI.Scripts.MVVM.View.Transition
{
    [CustomEditor(typeof(ITransition), true)]
    public class TransitionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Test", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Playing in Editor", TransitionEditorRunner.Transitioning ? "Yes" : "No");

            if (GUILayout.Button("Play (Editor)"))
            {
                TransitionEditorRunner.Play((ITransition)target);
            }

            if (GUILayout.Button("Reset (Editor)"))
            {
                TransitionEditorRunner.Reset((ITransition)target);
            }
        }

        private void OnDisable()
        {
            // 코루틴은 TransitionEditorRunner에서 관리하므로 이쪽에서는 Stop하지 않음
            // 만약 다른 리소스를 관리한다면 여기에 추가
        }
    }
}
