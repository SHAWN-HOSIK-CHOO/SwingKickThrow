using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STC
{
    public class STCDebugger : MonoBehaviour
    {
        [HideInInspector]
        public string treeState = "";

        private void FixedUpdate()
        {
            DumpStateTree();
        }
        public void DumpStateTree()
        {
            var builder = new System.Text.StringBuilder();
            DumpStateRecursive(StateChart.Instance, builder, "", true);
            treeState =  builder.ToString();
        }

        public static string GetStatusMark(bool IsActive,bool IsTask)
        {
            string status = "❌";
            if (IsActive == true)
            {
                status = "✅";
            }
            else if (IsTask)
            {
                status = "➖";
            }
            return status;
        }

        public static void DumpStateRecursive(IState state, System.Text.StringBuilder builder, string indent, bool isLast)
        {
            bool isTask = false;
            if (state.ParentState != null)
            {
                if (state.ParentState.CheckTask(state.Name))
                {
                    isTask = true;
                }
            }
            string branch = isLast ? "└── " : "├── ";
            builder.AppendLine($"{indent}{branch}{state.Name}{(GetStatusMark(state.Active, isTask))}");
            var children = state.GetAllChild();
            int i = 0;
            foreach (var child in children)
            {
                bool lastChild = (i == children.Count - 1);
                string newIndent = indent + (isLast ? "    " : "│   ");
                DumpStateRecursive(state.GetState(child.Key), builder, newIndent, lastChild);
                i++;
            }
        }
    }
}