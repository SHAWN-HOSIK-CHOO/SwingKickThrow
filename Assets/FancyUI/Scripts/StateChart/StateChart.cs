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
            public void Do();
        }
        public struct SetParentEvent : StateChartEvent
        {
            public IState state;
            public IState parent;
            public void Do()
            {
                if (state.ParentState != null)
                {
                    state.ParentState.DoTransitionFrom(state.Name);
                    state.ParentState.RemoveChild(state);
                }
                state.SetParent(parent);
                if (parent != null)
                {
                    parent.AddChild(state);
                    parent.DoTransitionTo(state.Name);
                }
            }
        }
        public struct AddChildEvent : StateChartEvent
        {
            public IState state;
            public IState child;
            public void Do()
            {
                if (child.ParentState != null)
                {
                    child.ParentState.DoTransition(child.Name,child.ParentState.Name);
                    child.ParentState.RemoveChild(child);
                }
                child.SetParent(state);
                state.AddChild(child);
                state.DoTransitionTo(child.Name);
            }
        }
        public struct RemoveChildEvent : StateChartEvent
        {
            public IState state;
            public IState child;
            public void Do()
            {
                state.DoTransitionFrom(child.Name);
                state.RemoveChild(child);
                child.SetParent(null);
            }
        }
        public struct TaskEvent : StateChartEvent
        {
            public IState state;
            public void Do()
            {
                if(StateChart.Instance.CheckPathValid(state))
                {
                    var fullPath = state.GetFullPath();
                    var pathParts = fullPath.Split('.');
                    IState currentState = Instance;
                    for (int i = 1; i < pathParts.Length; i++)
                    {
                        string name = pathParts[i];
                        if (!currentState.CheckTask(name))
                        {
                            currentState.DoTransitionTo(name);
                        }
                        currentState = currentState.GetState(name);
                    }
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
        private StateChart(string name) : base(name,StateType.Parallel)
        {
            EventList = new List<StateChartEvent>();
        }
        public bool CheckPathValid(IState state)
        {
            var fullPath = state.GetFullPath();
            var pathParts = fullPath.Split('.');
            IState currentState = state;
            for (int i = pathParts.Length-1; i > 0 ; i--)
            {
                string name = pathParts[i];
                if(!currentState.ParentState.CheckTransition(currentState.ParentState.Name, currentState.Name))
                {
                    return false;
                }
                currentState = currentState.ParentState;
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
                EventList.RemoveAt(i);
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