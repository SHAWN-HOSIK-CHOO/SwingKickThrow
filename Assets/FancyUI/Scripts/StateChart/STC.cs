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
            StateChart.Instance.Update();
        }

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
        private void DumpStateRecursive(IState state, System.Text.StringBuilder builder, string indent, bool isLast)
        {
            bool isActive = true;
            if (state.ParentState != null)
            {
                if (!state.ParentState.CheckTask(state.Name))
                {
                    isActive = false;
                }
            }

            string branch = isLast ? "└── " : "├── ";
            builder.AppendLine($"{indent}{branch}{state.Name}{(isActive ? "✅" : "❌")}");

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