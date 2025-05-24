using System.Collections.Generic;
using UnityEngine;
namespace STC
{
    public class State : IState
    {
        public string GetFullPath()
        {
            List<string> pathParts = new List<string>();
            IState? current = this;
            while (current != null)
            {
                pathParts.Add(current.Name);
                current = current.ParentState;
            }
            pathParts.Reverse();
            return string.Join(".", pathParts);
        }
        public bool Mute { get; protected set; }
        public string Name { get; protected set; }
        public IState ParentState { get; protected set; }
        public IState currentState { get; protected set; }
        protected Dictionary<string, IState> ChildState;
        public State(string name)
        {
            Name = name;
            currentState = this;
            ChildState = new Dictionary<string, IState>();
            ChildState.Add(Name,this);
        }
        public IState GetChildState(string name)
        {
            if(ChildState.TryGetValue(name,out var val ))
            {
                return val;
            }
            return null;
        }
        public Dictionary<string, IState>.KeyCollection GetAllChildState()
        {
            return ChildState.Keys;
        }
        public void Update()
        {
            if (currentState != this)
            {
                currentState.Update();
            }
            OnUpdate();
        }
        public void Enter()
        {
            if (currentState != this)
            {
                currentState.Enter();
            }
            OnEnter();
        }
        public void Exit()
        {
            if (currentState != this)
            {
                currentState.Exit();
            }
            OnExit();
        }
        public void RemoveChild(string name)
        {
            if(currentState.Name == name)
            {
                ChangeState(Name);
            }

            var chlid = GetChildState(name);
            if (chlid != null)
            {
                ChildState.Remove(name);
                chlid.DetachParent();
            }
        }
        public void DetachParent()
        {
            ParentState = null;
        }
        public void ChangeState(string name)
        {
            var chlid = GetChildState(name);
            if(chlid != null)
            {
                currentState.Exit();
                currentState = chlid;
                currentState.Enter();
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
    }
}