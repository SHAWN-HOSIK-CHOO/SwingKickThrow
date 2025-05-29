using MVVM.Property;
using MVVM.View.Window;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Window))]
public class WindowEditor : Editor
{
    Window window;
    SerializedProperty visibleProp;
    SerializedProperty interactProp;
    SerializedProperty transitionProp;
    SerializedProperty modalProp;
    private void OnEnable()
    {
        window = (Window)target;
        visibleProp = serializedObject.FindProperty("__visible");
        interactProp = serializedObject.FindProperty("__Interact");
        transitionProp = serializedObject.FindProperty("__Transition");
        modalProp = serializedObject.FindProperty("__Modal");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        if (window.Binder == null)
        {
            DrawBoolProperty("Visible", visibleProp);
            DrawBoolProperty("Interact", interactProp);
            DrawBoolProperty("Transition", transitionProp);
            DrawBoolProperty("Modal", modalProp);
        }
        else
        {
            var binding = window.Binder.ViewBinding;
            DrawBoundBool("Visible", binding.Visible, binding.VisibleVal);
            DrawBoundBool("Interact", binding.Interactable, binding.InteractableVal);
            DrawBoundBool("Transition", binding.Transition, binding.TransitionVal);
            DrawBoundBool("Modal", binding.Modal, binding.ModalVal);
        }

        serializedObject.ApplyModifiedProperties();
    }
    private void DrawBoundBool(string label, IPropertyNofier<bool> getter, IPropertySetter<bool> setter)
    {
        if (getter == null || setter == null)
            return;

        bool oldValue = getter.Get();
        bool newValue = EditorGUILayout.Toggle(label, oldValue);
        if (newValue != oldValue)
        {
            Undo.RecordObject(window, $"{label} Changed");
            setter.Value = newValue;
            EditorUtility.SetDirty(window);
        }
    }
    private void DrawBoolProperty(string label,SerializedProperty __property)
    {
        if (window.Binder == null)
        {
            EditorGUILayout.PropertyField(__property, new GUIContent(label));
        }
        else
        {
            bool newValue = EditorGUILayout.Toggle(label, window.Binder.ViewBinding.Visible.Get());
            if (newValue != window.Binder.ViewBinding.Visible.Get())
            {
                Undo.RecordObject(window, $"{label} Changed");
                window.Binder.ViewBinding.VisibleVal.Value = newValue;
                EditorUtility.SetDirty(window);
            }

        }
    }
}
