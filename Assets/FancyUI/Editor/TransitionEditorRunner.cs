using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using FancyUI.MVVM.View.Transition;

namespace FancyUI.Transition
{
    public static class TransitionEditorRunner
    {
        private static double StartTime;
        private static double CurrentTime;
        public static bool Transitioning { get; private set; }

        private static EditorCoroutine Coroutine;

        public static void Play(ITransition transition)
        {
            Stop();

            Transitioning = true;
            transition.Initialize();
            Coroutine = EditorCoroutineUtility.StartCoroutine(UpdateTime(transition), EditorWindow.GetWindow<SceneView>());
        }

        public static void Reset(ITransition transition)
        {
            Stop();

            transition.Initialize();
            transition.Apply(0);
        }

        public static void Stop()
        {
            if (Coroutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(Coroutine);
                Coroutine = null;
            }

            Transitioning = false;
        }

        private static IEnumerator UpdateTime(ITransition transition)
        {
            StartTime = EditorApplication.timeSinceStartup;
            while (Transitioning)
            {
                CurrentTime = EditorApplication.timeSinceStartup;
                var elapsed = CurrentTime - StartTime;

                transition.Apply(Utils.Math.Math.Normalize(transition.duration, elapsed));

                if (elapsed >= transition.duration)
                {
                    Transitioning = false;
                    Coroutine = null;
                    yield break;
                }

                SceneView.RepaintAll();
                yield return null;
            }
        }
    }
}
