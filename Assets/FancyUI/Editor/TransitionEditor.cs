using UnityEditor;
using UnityEngine;

namespace FancyUI
{
    public class TransitionEditor<T> : Editor where T : MonoBehaviour, ITransition
    {
        private bool isPlayingInEditor = false;
        private float editorElapsed = 0f;
        private double lastTime = 0f;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Test", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Playing in Editor", isPlayingInEditor ? "Yes" : "No");
            if (GUILayout.Button("Play (Editor)"))
            {
                StartPlayInEditor((T)target);
            }
        }

        private void StartPlayInEditor(ITransition transition)
        {
            isPlayingInEditor = true;
            editorElapsed = 0f;
            lastTime = EditorApplication.timeSinceStartup;
            if (Application.isPlaying)
            {
                transition.Initialize();
                transition.Play(); // Coroutine 실행 (Play 모드)
            }
            else
            {
                transition.Initialize();
                EditorApplication.update += EditorUpdate; // Edit 모드 보간용
            }
        }

        private void EditorUpdate()
        {
            double currentTime = EditorApplication.timeSinceStartup;
            editorElapsed = (float)(currentTime - lastTime);
            ITransition transition = (T)target;
            if (!transition.Apply(transition.duration, editorElapsed))
            {
                isPlayingInEditor = false;
                EditorApplication.update -= EditorUpdate;
            }
            SceneView.RepaintAll();
            Repaint();
        }
    }
}