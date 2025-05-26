using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STC
{
    public class STC : MonoBehaviour
    {
        [HideInInspector]
        public string treeState = "";
        void Update()
        {
#if UNITY_EDITOR
            if(StateChart.Instance.GetEventListNum() > 0)
            {
                treeState = DumpStateTree();
            }
#endif
            StateChart.Instance.Update();
        }
        public string DumpStateTree()
        {
            var builder = new System.Text.StringBuilder();
            DumpStateRecursive(StateChart.Instance, builder, "", true);
            return builder.ToString();
        }
        private void DumpStateRecursive(IState state, System.Text.StringBuilder builder, string indent, bool isLast)
        {
            bool isActive = true;
            if (state.ParentState != null)
            {
                if (!state.ParentState.GetAllTaskState().Contains(state.Name))
                {
                    isActive = false;
                }
            }

            string branch = isLast ? "└── " : "├── ";
            builder.AppendLine($"{indent}{branch}{state.Name}{(isActive ? "✅" : "❌")}");

            var children = state.GetAllChildState().ToList();
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                bool lastChild = (i == children.Count - 1);
                string newIndent = indent + (isLast ? "    " : "│   ");
                DumpStateRecursive(state.GetChildState(child), builder, newIndent, lastChild);
            }
        }
    }
}