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
                    state.ParentState.RemoveChild(state);
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
            public void Do()
            {
                if (child.ParentState != null)
                {
                    child.ParentState.RemoveChild(child);
                }
                child.SetParent(state);
                state.AddChild(child);
            }
        }
        public struct RemoveChildEvent : StateChartEvent
        {
            public IState child;
            public void Do()
            {
                if (child.ParentState != null)
                {
                    child.ParentState.RemoveChild(child);
                }
                child.SetParent(null);
            }
        }
        public struct TansitionEvent : StateChartEvent
        {
            public IState State;
            public TansitionEvent(IState state)
            {
                State = state;
            }
            public void Do()
            {
                State.DoTransition();
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
            Active = true;
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