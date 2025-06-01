using STC;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
[CustomEditor(typeof(STC.STCDebugger))]
public class STCDebuggerEditor : Editor
{
    STC.STCDebugger stc;
    private Vector2 scrollPos;

    private void OnEnable()
    {
        stc = (STC.STCDebugger)target;
    }
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Show State Tree"))
        {
            stc.DumpStateTree();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("State Tree:", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
        EditorGUILayout.TextArea(stc.treeState, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }
}
