using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace STC
{
    public class StateChart : State
    {
        public interface StateChartEvent
        {
            bool ReDo { get; }
            bool ForceActivatePath { get; }
            public void Do();
        }
        public struct SetParentEvent : StateChartEvent
        {
            public IState state;
            public IState parent;
            public ICondition condition;
            public bool ReDo { get; set; }

            public bool ForceActivatePath { get; set; } // NotUsed
            public void Do()
            {
                if (!condition.CheckCondition())
                {
                    return;
                }

                if (parent != null)
                {
                    parent.RemoveChild(state);
                }
                state.SetParent(parent);
                if (parent != null)
                {
                    parent.AddChild(state);
                }
            }
        }
        public struct AddChildEvent : StateChartEvent
        {
            public IState state;
            public IState child;
            public ICondition condition;
            public bool ForceActivatePath { get; set; } // NotUsed

            public bool ReDo { get; set; }
            public void Do()
            {
                if (!condition.CheckCondition())
                {
                    return;
                }
                if (child.ParentState != null)
                {
                    child.ParentState.RemoveChild(state);
                }
                child.SetParent(state);
                state.AddChild(child);
            }
        }
        public struct RemoveChildEvent : StateChartEvent
        {
            public IState state;
            public IState child;
            public ICondition condition;

            public bool ReDo { get; set; }
            public bool ForceActivatePath { get; set; } // NotUsed
            public void Do()
            {
                if (!condition.CheckCondition())
                {
                    return;
                }
                state.RemoveChild(child);
                child.SetParent(null);
            }
        }
        public struct TaskInEvent : StateChartEvent
        {
            public IState task;
            public ICondition condition;
            public bool ReDo { get; set; }
            public bool ForceActivatePath { get; set; }
           
            public void Do()
            {
                if (!condition.CheckCondition())
                {
                    return;
                }

                if (task.ParentState != null)
                {
                    if (ForceActivatePath)
                    {
                        StateChart.Instance.ForceActivatePath(task.ParentState);
                    }
                    if (!StateChart.Instance.CheckPathActive(task.ParentState))
                    {
                        return;
                    }
                    task.ParentState.TaskInChlid(task.Name);
                }
            }
        }
        public struct TaskOutEvent : StateChartEvent
        {
            public IState task;
            public ICondition condition;
            public bool ReDo { get; set; }
            public bool ForceActivatePath { get; set; }

            public void Do()
            {
                if (!condition.CheckCondition())
                {
                    return;
                }
                if (task.ParentState != null)
                {
                    if (ForceActivatePath)
                    {
                        StateChart.Instance.ForceActivatePath(task.ParentState);
                    }
                    if (!StateChart.Instance.CheckPathActive(task.ParentState))
                    {
                        return;
                    }
                    task.ParentState.TaskOutChlid(task.Name);
                }

            }
        }
        public struct TaskEvent : StateChartEvent
        {
            public IState taskIn;
            public IState taskOut;
            public ICondition condition;
            public bool ReDo { get; set; }
            public bool ForceActivatePath { get; set; }
            public void Do()
            {
                if (!condition.CheckCondition())
                {
                    return;
                }
                if (taskIn.ParentState != null)
                {
                    if (taskIn.ParentState != taskOut.ParentState) return;
                    if (ForceActivatePath)
                    {
                        StateChart.Instance.ForceActivatePath(taskIn.ParentState);
                    }
                    if (!StateChart.Instance.CheckPathActive(taskIn.ParentState))
                    {
                        return;
                    }
                    taskIn.ParentState.TaskChlid(taskIn.Name, taskOut.Name);
                }
            }
        }
        private static StateChart _instance;
        public static StateChart Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StateChart("Origin");
                }
                return _instance;
            }
        }
        private List<StateChartEvent> EventList;
        private StateChart(string name) : base(name)
        {
            EventList = new List<StateChartEvent>();
        }
        public void ForceActivatePath(IState state)
        {  
            var fullPath = state.GetFullPath();
            var pathParts = fullPath.Split('.');
            IState currentState = this;
            for (int i = 1; i < pathParts.Length; i++)
            {
                string childName = pathParts[i];
                var chlidState = currentState.GetChildState(childName);
                if (chlidState == null)
                {
                    throw new System.Exception($"Child state '{childName}' not found in state '{currentState.Name}'");
                }
                currentState.TaskInChlid(chlidState.Name);
                currentState = chlidState;
            }
        }
        public bool CheckPathActive(IState state)
        {
            var fullPath = state.GetFullPath();
            var pathParts = fullPath.Split('.');
            IState currentState = this;
            for (int i = 1; i < pathParts.Length; i++)
            {
                string childName = pathParts[i];
                var taskState = currentState.GetAllTaskState();
                if (!taskState.Contains(childName))
                {
                    return false;
                }
                var chlidState = currentState.GetChildState(childName);
                currentState = chlidState;
            }
            return true;
        }
        public void PublishEvent(StateChartEvent e)
        {
            EventList.Add(e);
        }
        public void FlushEventList()
        {
            for (int i = 0 ; i < EventList.Count; i++)
            {
                var e = EventList[i];
                e.Do();
            }
            for (int i = EventList.Count - 1; i >= 0; i--)
            {
                var e = EventList[i];
                if (!e.ReDo)
                {
                    EventList.RemoveAt(i);
                }
            }
        }
        public override void OnUpdate()
        {
            FlushEventList();
        }
    
        public int GetEventListNum()
        {
            return EventList.Count;
        }
    }
}