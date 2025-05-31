using MVVM.Property;
using MVVM.ViewModel.Window.State;
using UnityEditor;
using UnityEngine;

namespace MVVM.ViewModel.Window
{
    [CustomEditor(typeof(MVVM.View.Window.Window))]
    public class WindowEditor : Editor
    {
        MVVM.View.Window.Window window;
        SerializedProperty visibleProp;
        SerializedProperty interactProp;
        SerializedProperty transitionProp;
        SerializedProperty modalProp;
        SerializedProperty modalWindowProp;
        SerializedProperty childWindowListProp;
        private void OnEnable()
        {
            window = (MVVM.View.Window.Window)target;
            visibleProp = serializedObject.FindProperty("__visible");
            interactProp = serializedObject.FindProperty("__Interact");
            transitionProp = serializedObject.FindProperty("__Transition");
            modalProp = serializedObject.FindProperty("__Modal");
            childWindowListProp = serializedObject.FindProperty("__ChildWindowList");
            modalWindowProp = serializedObject.FindProperty("__ModalWindow");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            if (!Application.isPlaying)
            {
                EditorGUILayout.PropertyField(modalWindowProp);
                EditorGUILayout.PropertyField(childWindowListProp);
            }
            else
            {
                foreach (var kvp in  window.Binder?.WindowHierachyState.GetAllChildState())
                {
                    if(Equals(window.Binder?.WindowHierachyState?.ModalHierachy, kvp.Value))
                    {
                        EditorGUILayout.LabelField("Modal Window : " + kvp.Key, EditorStyles.boldLabel);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Child Window : " + kvp.Key, EditorStyles.boldLabel);
                    }
                }
            }

            var trigger = window.Binder?.ViewTrigger;
            var binder = window.Binder?.ViewBinding;

            if (!DrawBoundBool("Visible", binder?.Visible, trigger?.VisibleTrigger))
            {
                DrawBoolProperty("Visible", visibleProp);
            }

            if (!DrawBoundBool("Interact", binder?.Interactable,  trigger?.InteractableTrigger))
            {
                DrawBoolProperty("Interact", interactProp);
            }

            if (!DrawBoundBool("Transition", binder?.Transition, trigger?.TransitionTrigger))
            {
                DrawBoolProperty("Transition", transitionProp);
            }
            if (!DrawBoundBool("Modal", binder?.Modal, trigger?.ModalTrigger))
            {
                DrawBoolProperty("Modal", modalProp);
            }
            serializedObject.ApplyModifiedProperties();
        }
        private bool DrawBoundBool(string label, IPropertyNofier<bool> getter, IPropertySetter<bool> setter)
        {
            if (getter == null || setter == null)
            {
                return false;
            }

            using (new GUILayout.HorizontalScope())
            {
                // 1. 현재 값 보여주기 (읽기 전용)
                EditorGUILayout.LabelField(label, GUILayout.Width(100)); // 너비 고정
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle(getter.Get(), GUILayout.Width(20));
                EditorGUI.EndDisabledGroup();

                // 2. 트리거용 토글 버튼 (라벨과 토글 분리)
                GUILayout.Label("Trigger", GUILayout.Width(50));
                var oldValue = EditorGUILayout.Toggle(setter.Value, GUILayout.Width(20));
                if (oldValue != setter.Value)
                {
                    setter.Value = oldValue;
                    Undo.RecordObject(window, $"{label} Triggered");
                    EditorUtility.SetDirty(window);
                }
            }
            return true;
        }
        private void DrawBoolProperty(string label, SerializedProperty __property)
        {
           EditorGUILayout.PropertyField(__property, new GUIContent(label));
        }
    }
}