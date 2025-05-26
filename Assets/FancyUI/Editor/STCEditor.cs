using STC;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
[CustomEditor(typeof(STC.STC))]
public class STCEditor : Editor
{
    private Vector2 scrollPos;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var stc = (STC.STC)target;
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
