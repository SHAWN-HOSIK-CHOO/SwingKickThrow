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
        DrawBoolProperty("Visible", window.Visible, visibleProp);
        DrawBoolProperty("Interact", window.Interact, interactProp);
        DrawBoolProperty("Transition", window.Transition, transitionProp);
        DrawBoolProperty("Modal", window.Modal, modalProp);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawBoolProperty(string label, BoolProperty property, SerializedProperty __property)
    {
        if (property == null)
        {
            EditorGUILayout.PropertyField(__property, new GUIContent(label));
        }
        else
        {
            bool newValue = EditorGUILayout.Toggle(label, property.Value);
            if (newValue != property.Value)
            {
                Undo.RecordObject(window, $"{label} Changed");
                property.Value = newValue;
                EditorUtility.SetDirty(window);
            }
        }
    }
}
