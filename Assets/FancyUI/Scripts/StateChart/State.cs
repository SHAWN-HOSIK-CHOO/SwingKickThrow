using CharacterAttributes;
using STC.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.Rendering;
using static STC.StateChart;
namespace STC
{
    public class State : IState
    {

        public enum StateType
        {
            FSM,
            Parallel
        }
        public bool Active { get; protected set; }
        public bool Task { get; protected set; }
        public StateType Type { get; protected set; }
        public string Name { get; protected set; }
        public IState ParentState { get; protected set; }

        //Fsm
        protected IState FSMTask;
        protected Dictionary<string, IState> FSMState;
        protected DoubleKeyDict<string, string, ITransition> FSMTransition;
        //Parallel
        protected Dictionary<string, IState> ParallelTask;
        protected Dictionary<string, IState> ParallelState;
        protected DoubleKeyDict<string, string, ITransition> ParallelTransition;

        public State(string name, StateType type)
        {
            Name = name;
            Type = type;

            if (type == StateType.FSM)
            {
                FSMState = new Dictionary<string, IState>();
                FSMTransition = new DoubleKeyDict<string, string, ITransition>();
            }
            else
            {
                ParallelTask = new Dictionary<string, IState>();
                ParallelState = new Dictionary<string, IState>();
                ParallelTransition = new DoubleKeyDict<string, string, ITransition>();
            }
        }
        public IState GetFSMTask()
        {
            if (Type == StateType.FSM)
            {
                if (FSMTask == null)
                {
                    return this;
                }
                return FSMTask;
            }
            return null;
        }
        public IState GetChildState(string name)
        {
            if (Type == StateType.FSM)
            {
                return GetChildFSMState(name);
            }
            else
            {
                return GetChildParallelState(name);
            }
        }
        public IState GetChildFSMState(string name)
        {
            if (FSMState.TryGetValue(name, out var val))
            {
                return val;
            }
            return null;
        }
        public IState GetChildParallelState(string name)
        {
            if (ParallelState.TryGetValue(name, out var val))
            {
                return val;
            }
            return null;
        }
        public bool IsTask(string childName)
        {
            if(Type == StateType.FSM)
            {
                if (FSMTask != null && FSMTask.Name == childName)
                {
                    return true;
                }
            }
            else
            {
                if (ParallelTask.ContainsKey(childName))
                {
                    return true;
                }
            }
            return false;
        }

        private bool DoFSNTransition(string src, string dst)
        {
            var transition = GetTransition(src, dst);
            var srcState = GetFSMTask();
            if (srcState.Name != src) return false;
            var dstState = dst == Name ? this : GetChildFSMState(dst);
            if (dstState == null)
            {
                CheckTransitionBarrier(src);
            }
            else
            {
                dstState.CheckTransitionBarrier(src);
            }

            if (!transition.Condition.CheckValid()) return false;

            if (src == Name)
            {
                var newState = GetChildFSMState(dst);
                FSMTask = newState;
                FSMTask.Enter();
                if (Active)
                {
                    FSMTask.Activate();
                }
                return true;
            }
            else if (dst == Name)
            {
                var state = GetChildFSMState(src);
                if (FSMTask == state)
                {
                    FSMTask = null;
                    state.Exit();
                    if (state.Active)
                    {
                        state.InActivate();
                    }
                    return true;
                }
            }
            else
            {
                FSMTask = null;
                srcState.Exit();
                if (srcState.Active)
                {
                    srcState.InActivate();
                }
                FSMTask = dstState;
                FSMTask.Enter();
                if (Active)
                {
                    FSMTask.Activate();
                }
                return true;
            }
            return false;
        }
        private bool DoParallelTransition(string src, string dst)
        {
            var transition = GetTransition(src, dst);
            if (src == Name)
            {
                if (!ParallelTask.TryGetValue(dst, out var _))
                {
                    var dstState = GetChildParallelState(dst);
                    dstState.CheckTransitionBarrier(dst);
                    if (transition.Condition.CheckValid())
                    {
                        ParallelTask.Add(dst, dstState);
                        dstState.Enter();
                        if (Active)
                        {
                            dstState.Activate();
                        }
                        return true;
                    }
                }
            }
            else if (dst == Name)
            {
                if (ParallelTask.TryGetValue(src, out var srcState))
                {
                    CheckTransitionBarrier(dst);
                    if (transition.Condition.CheckValid())
                    {
                        ParallelTask.Remove(src);
                        srcState.Exit();
                        if (srcState.Active)
                        {
                            srcState.InActivate();
                        }
                        return true;
                    }
                }
            }

            return false;
        }
        private bool DoTransition(string src, string dst)
        {
            if (Type == StateType.FSM)
            {
                return DoFSNTransition(src, dst);
            }
            else
            {
                return DoParallelTransition(src, dst);   
            }
        }
        public bool DoTransition()
        {
            bool trans = false;
            if (Type == StateType.FSM)
            {
                foreach (var (key1, key2, transition) in FSMTransition.GetEnumerator())
                {
                    if (DoTransition(transition.Src, transition.Dst))
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (var (key1, key2, transition) in ParallelTransition.GetEnumerator())
                {
                    if (DoTransition(transition.Src, transition.Dst))
                    {
                        trans = true;
                    }
                }
            }
            return trans;
        }
        public void CheckTransitionBarrier(string src)
        {
            OnCheckTransitionBarrier(src);
        }
        public void Activate()
        {
            Active = true;
            OnActivate();
            if(!DoTransition())
            {
                if (Type == StateType.FSM)
                {
                    if (FSMTask != null)
                    {
                        var state = FSMTask;
                        state.Activate();
                    }
                }
                else
                {
                    foreach (var key in ParallelTask.Keys.ToList())
                    {
                        var state = GetChildParallelState(key);
                        state.Activate();
                    }
                }
            }
        }
        public void Enter()
        {
            Task = true;
            OnEnter();
        }
        public void Update()
        {
            OnUpdate();
            if (!DoTransition())
            {
                if (Type == StateType.FSM)
                {
                    if (FSMTask != null)
                    {
                        var state = FSMTask;
                        if (state.Active)
                        {
                            state.Update();
                        }
                    }
                }
                else
                {
                    foreach (var key in ParallelTask.Keys.ToList())
                    {
                        var state = GetChildParallelState(key);
                        if (state.Active)
                        {
                            state.Update();
                        }
                    }
                }
            }
        }
        public void Exit()
        {
            Task = false; 
            OnExit();
        }
        public void InActivate()
        {
            if (Type == StateType.FSM)
            {
                if (FSMTask != null)
                {
                    var state = FSMTask;
                    state.InActivate();
                }
            }
            else
            {
                foreach (var key in ParallelTask.Keys.ToList())
                {
                    var state = GetChildParallelState(key);
                    state.InActivate();
                }
            }

            Active = false;
            OnInActivate();
        }
        private bool CanHandle(IStateEvent e)
        {
            bool checkValid = false;
            if (ParentState != null)
            {
                if (ParentState.Type == StateType.FSM)
                {
                    if (!e.Consumed.isConsumed)
                    {
                        checkValid = true;
                    }
                }
                else
                {
                    checkValid = true;
                }
            }
            else
            {
                checkValid = true;
            }
            if (e.RequireActiveState && !Active)
            {
                checkValid = false;
            }
            return checkValid;

        }
        public void Handle(IStateEvent e)
        {
            if (CanHandle(e))
            {
                if (Type == StateType.FSM)
                {
                    if (e.DownStream)
                    {
                        OnHandle(e);
                        foreach (var state in FSMState)
                        {
                            state.Value.Handle(e);
                            if(e.Consumed.isConsumed)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {

                        foreach (var state in FSMState)
                        {
                            state.Value.Handle(e);
                        }
                        if (CanHandle(e))
                        {
                            OnHandle(e);
                        }
                    }
                }
                else
                {
                    if (e.DownStream)
                    {
                        OnHandle(e);
                        foreach (var state in ParallelState)
                        {
                            state.Value.Handle(e);
                            if (e.Consumed.isConsumed)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var state in ParallelState)
                        {
                            state.Value.Handle(e);
                        }
                        OnHandle(e);
                    }
                }
            }
        }
        public virtual void OnHandle(global::STC.Event.IStateEvent e)
        {

        }
        public virtual void OnSetParent(IState state)
        {

        }
        public virtual void OnAddChlid(IState state)
        {

        }
        public virtual void OnRemoveChlid(IState state)
        {

        }
        public virtual void OnCheckTransitionBarrier(string src)
        {

        }
        public virtual void OnActivate()
        {

        }
        public virtual void OnEnter()
        {

        }
        public virtual void OnUpdate()
        {

        }
        public virtual void OnExit()
        {

        }
        public virtual void OnInActivate()
        {

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
                if (current == null)
                {
                    return null;
                }
            }
            return current;
        }
        public void SetParent(IState state)
        {
            ParentState = state;
            OnSetParent(state);

        }
        public void AddChild(IState state)
        {
            if (Type == StateType.FSM)
            {
                FSMState.Add(state.Name, state);
                OnAddChlid(state);
            }
            else
            {
                ParallelState.Add(state.Name, state);
                OnAddChlid(state);
            }
        }
        public void RemoveChild(IState state)
        {
            if (state.Type == StateType.FSM)
            {
                FSMState.Remove(state.Name);
                OnRemoveChlid(state);
            }
            else
            {
                ParallelState.Remove(state.Name);
                OnRemoveChlid(state);
            }
        }
        public ITransition GetTransition(string src, string dst)
        {
            if (Type == StateType.FSM)
            {
                if(FSMTransition.TryGetValue(src, dst, out var val))
                {
                    return val;
                }
            }
            else
            {
                if(ParallelTransition.TryGetValue(src, dst, out var val))
                {
                    return val;
                }
            }
            return null;
        }
        public void RemoveTransition(string src, string dst)
        {
            if (Type == StateType.FSM)
            {
                FSMTransition.Remove(src, dst);
            }
            else
            {
                ParallelTransition.Remove(src, dst);
            }
        }
        public void AddTransition(ITransition transition)
        {
            if (Type == StateType.FSM)
            {
                FSMTransition.TryAdd(transition.Src, transition.Dst, transition);
            }
            else
            {
                ParallelTransition.TryAdd(transition.Src, transition.Dst, transition);
            }
            transition.ConditionValidSignal += TransitionDirty;
        }
        void TransitionDirty(ITransition transition)
        {
            StateChart.Instance.PublishEvent(new TansitionEvent(this));
        }
        public Dictionary<string, IState> GetAllChildState()
        {
            if (Type == StateType.FSM)
            {
                return FSMState;
            }
            else
            {
                return ParallelState;
            }
        }
    }
}