using System.Collections.Generic;
using UnityEngine;
namespace STC
{
    public class State : IState
    {
        public string Name { get; protected set; }
        public IState ParentState { get; protected set; }
        //Child State Chart
        protected Dictionary<string, IState> Task;
        protected Dictionary<string, IState> ChildState;
        public State(string name)
        {
            Name = name;
            Task = new Dictionary<string, IState>();
            ChildState = new Dictionary<string, IState>();
        }
        public IState GetChildState(string name)
        {
            if(ChildState.TryGetValue(name,out var val ))
            {
                return val;
            }
            return null;
        }
        public void Update()
        {
            OnUpdate();

            foreach (var kvp in Task)
            {
                kvp.Value.Update();
            }
        }
        public void Enter()
        {
            OnEnter();
            foreach (var kvp in Task)
            {
                kvp.Value.Enter();
            }
        }
        public void Exit()
        {
            OnExit();
            foreach (var kvp in Task)
            {
                kvp.Value.Exit();
            }
        }
        public virtual void OnEnter()
        {

        }
        public virtual void OnExit()
        {

        }
        public virtual void OnUpdate()
        {

        }
        public Dictionary<string, IState>.KeyCollection GetAllTaskState()
        {
            return Task.Keys;
        }
        public Dictionary<string, IState>.KeyCollection GetAllChildState()
        {
            return ChildState.Keys;
        }
        public string GetFullPath()
        {
            List<string> pathParts = new List<string>();
            IState current = this;
            while (current != null)
            {
                pathParts.Add(current.Name);
                current = current.ParentState;
            }
            pathParts.Reverse();
            return string.Join(".", pathParts);
        }
        public void TaskInChlid(string name)
        {
            if(Task.TryGetValue(name,out var val))
            {
                val.Exit();
                Task.Remove(name);
            }
            var state = GetChildState(name);
            if(state != null)
            {
                Task.Add(name, state);
                state.Enter();
            }
        }
        public void TaskOutChlid(string name)
        {
            if (Task.TryGetValue(name, out var val))
            {
                val.Exit();
                Task.Remove(name);
            }
        }
        public void TaskChlid(string inTaskState, string outTaskState)
        {
            TaskOutChlid(outTaskState);
            TaskInChlid(inTaskState);
        }
        public void SetParent(IState state)
        {
            ParentState = state;
        }
        public void AddChild(IState state)
        {
            ChildState.Add(state.Name,state);
            state.SetParent(this);
        }
        public void RemoveChild(IState state)
        {
            ChildState.Remove(state.Name);
            state.SetParent(null);
        }
    }
}