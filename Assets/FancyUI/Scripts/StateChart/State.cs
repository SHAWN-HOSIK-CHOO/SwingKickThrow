using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
namespace STC
{
    public class State : IState
    {
        public string Name { get; protected set; }
        public IState ParentState { get; protected set; }
        public event Action<IState> OnStateSetUp;
        public event Action<IState> OnStateEnter;
        public event Action<IState> OnStateExit;
        public event Action<IState> OnStateUpdate;
        public event Action<IState> OnStateFinish;
        //Child State Chart
        protected Dictionary<string, IState> Task;
        protected Dictionary<string, IState> TaskOutMap;
        protected Dictionary<string, IState> ChildState;

        public State(string name)
        {
            Name = name;
            Task = new Dictionary<string, IState>();
            TaskOutMap = new Dictionary<string, IState>();
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
        public void SetUp()
        {
            OnSetUp();
            OnStateSetUp?.Invoke(this);
        }
        public void Enter()
        {
            OnEnter();
            OnStateEnter?.Invoke(this);
            foreach (var kvp in Task.ToList())
            {
                if (Task.ContainsKey(kvp.Key))
                {
                    kvp.Value.Enter();
                }
            }
        }
        public void Update()
        {
            OnUpdate();
            OnStateUpdate?.Invoke(this);
            foreach (var kvp in Task.ToList())
            {
                if (Task.ContainsKey(kvp.Key))
                {
                    kvp.Value.Update();
                }
            }
        }
        public void Exit()
        {
            foreach (var kvp in Task.ToList())
            {
                if (Task.ContainsKey(kvp.Key))
                {
                    Task.Remove(kvp.Key);
                    kvp.Value.Exit();
                    kvp.Value.Finish();
                }
            }
            OnExit();
            OnStateExit?.Invoke(this);
        }
        public void Finish()
        {
            OnFinish();
            OnStateFinish?.Invoke(this);
        }
        public void Handle(global::STC.Event.IStateEvent e)
        {
            if(ParentState !=null)
            {
                if(ParentState.GetAllTaskState().Contains(Name))
                {
                    OnHandle(e);
                }
            }
        }
        public virtual void OnHandle(global::STC.Event.IStateEvent e)
        {

        }
        public virtual void OnSetUp()
        {

        }
        public virtual void OnEnter()
        {

        }
        public virtual void OnExit()
        {

        }
        public virtual void OnFinish()
        {

        }
        public virtual void OnUpdate()
        {

        }
        public List<string> GetAllTaskState()
        {
            return Task.Keys.ToList();
        }
        public List<string> GetAllChildState()
        {
            return ChildState.Keys.ToList();
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
        public IState FindChlid(string relativePath)
        {
            string[] pathParts = relativePath.Split('.');
            IState current = this;
            foreach (string part in pathParts)
            {
                current = current.GetChildState(part);
                if(current == null)
                {
                    return null;
                }
            }
            return current;
        }
        public void TaskInChlid(string name)
        {
            var state = GetChildState(name);
            if(state != null)
            {
                foreach (var v in state.GetTaskOutMap())
                {
                    if (Task.TryGetValue(v, out var t))
                    {
                        Task.Remove(t.Name);
                        t.Exit();
                        t.Finish();
                    }
                }

                if (Task.TryGetValue(name, out var val))
                {
                    Task.Remove(name);
                    val.Exit();
                    val.Finish();
                }


                state.SetUp();
                Task.Add(name, state);
                state.Enter();
            }
        }
        public void TaskOutChlid(string name)
        {
            if (Task.TryGetValue(name, out var val))
            {
                Task.Remove(name);
                val.Exit();
                val.Finish();
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
        public void AddTaskOutMap(IState state)
        {
            TaskOutMap.Add(state.Name, state);
        }
        public void RemoveTaskOutMap(IState state)
        {
            if (TaskOutMap.TryGetValue(state.Name, out var val))
            {
                TaskOutMap.Remove(state.Name);
            }
        }
        public void AddChild(IState state)
        {
            ChildState.Add(state.Name,state);
            state.SetParent(this);
        }
        public void RemoveChild(IState state)
        {
            TaskOutChlid(state.Name);
            ChildState.Remove(state.Name);
            state.SetParent(null);
        }
        public List<string> GetTaskOutMap()
        {
            return TaskOutMap.Keys.ToList();
        }
    }
}