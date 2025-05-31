using MVVM.ViewModel.Window.State;
using STC;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace STC
{
    public class WindowSTCDebugger : MonoBehaviour
    {
        public MVVM.View.Window.Window window;

        [HideInInspector]
        public string treeState = "";

        private void FixedUpdate()
        {
            DumpStateTree();
        }
        public void DumpStateTree()
        {
            if(window.Binder?.WindowHierachyState != null)
            {
                var builder = new System.Text.StringBuilder();
                DumpStateRecursive(window.Binder.WindowHierachyState, builder, "", true);
                treeState = builder.ToString();
            }
        }

        public void DumpStateRecursive(IState state, System.Text.StringBuilder builder, string indent, bool isLast)
        {
            bool isTask = false;
            if (state.ParentState != null)
            {
                if (state.ParentState.IsTask(state.Name))
                {
                    isTask = true;
                }
            }
            string branch = isLast ? "└── " : "├── ";
            builder.AppendLine($"{indent}{branch}{state.Name}{(STC.STCDebugger.GetStatusMark(state.Active, isTask))}");
            if (state is WindowHierachyState windowHierachy)
            {
                string newIndent = indent + (isLast ? "    " : "│   ");
                string vmBranch = "├── "; 
                builder.AppendLine($"{newIndent}{vmBranch}-ViewModel-");
                string vmIndent = newIndent + "│   ";
                STCDebugger.DumpStateRecursive(windowHierachy.ViewModel, builder, vmIndent, true);
            }
            var children = state.GetAllChildState();
            int i = 0;
            foreach (var child in children)
            {
                bool lastChild = (i == children.Count - 1);
                string newIndent = indent + (isLast ? "    " : "│   ");
                DumpStateRecursive(state.GetChildState(child.Key), builder, newIndent, lastChild);
                i++;
            }
        }
    }
}