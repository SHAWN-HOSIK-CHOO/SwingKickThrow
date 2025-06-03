using FancyUI.MVVM.View.Transition;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Utils.Math;

namespace FancyUI.Scripts.MVVM.View.Transition
{
    public class TransitionEditor : Editor 
    {
        double StartTime;
        double CurrentTime;
        bool Transitioning;

        EditorCoroutine Coroutine;


        private void OnDisable()
        {
            if (Coroutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(Coroutine);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Test", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Playing in Editor", Transitioning ? "Yes" : "No");
            if (GUILayout.Button("Play (Editor)"))
            {
                StartPlayInEditor();
            }
        }

         void StartPlayInEditor()
        {
            ITransition transition = (ITransition)target;
            if (Coroutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(Coroutine);
            }

            Transitioning = true;
            StartTime = EditorApplication.timeSinceStartup;
            CurrentTime = EditorApplication.timeSinceStartup;
            transition.Initialize();
            Coroutine = EditorCoroutineUtility.StartCoroutine(UpdateTime(), this);
        }

        private IEnumerator UpdateTime()
        {
            while (Transitioning)
            {
                ITransition transition = (ITransition)target;
                CurrentTime = EditorApplication.timeSinceStartup;
                var elapsed = CurrentTime - StartTime;
                transition.Apply(Utils.Math.Math.Normalize(transition.duration, elapsed));
                if (elapsed >= transition.duration)
                {
                    Transitioning = false;
                }
                SceneView.RepaintAll();
                Repaint();
                yield return null;
            }
        }
    }
}