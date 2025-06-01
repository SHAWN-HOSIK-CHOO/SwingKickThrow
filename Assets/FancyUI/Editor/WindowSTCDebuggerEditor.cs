using STC;
using UnityEditor;
using UnityEngine;

namespace STC
{
    [CustomEditor(typeof(STC.WindowSTCDebugger))]
    public class WindowSTCDebuggerEditor : Editor
    {
        STC.WindowSTCDebugger stc;
        private Vector2 scrollPos;
        private void OnEnable()
        {
            stc = (STC.WindowSTCDebugger)target;
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

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
}