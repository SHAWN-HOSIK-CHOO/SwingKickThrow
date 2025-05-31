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
        /*
            상태 구조는 계층, 병렬, 수평적으로 쌓을 수 있습니다.
            Transition을 통해 상태 전이에 대한 조건과 정의를 해야하며 



            상태는 매 Update 호출 마다 하위 Transition 으로의 전이를 수행합니다. 
            전이 이전에 BeforeCheckTransition 메서드가 호출되며,









            Transition 은 

         */



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

        public event Action<IState> OnStateActivate;
        public event Action<IState> OnStateEnter;
        public event Action<IState> OnStateUpdate;
        public event Action<IState> OnStateExit;
        public event Action<IState> OnStateInActivate;

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
        public string GetFSMTask()
        {
            if (Type == StateType.FSM)
            {

                if (FSMTask == null)
                {
                    return Name;
                }
                return FSMTask.Name;
            }
            return null;
        }
        public IState GetState(string name)
        {
            if (Type == StateType.FSM)
            {
                return GetFSMState(name);
            }
            else
            {
                return GetParallelState(name);
            }
        }
        public IState GetFSMState(string name)
        {
            if (FSMState.TryGetValue(name, out var val))
            {
                return val;
            }
            return null;
        }
        public IState GetParallelState(string name)
        {
            if (ParallelState.TryGetValue(name, out var val))
            {
                return val;
            }
            return null;
        }
        public bool CheckTransitionValid(string src, string dst)
        {
            if (CheckTask(src) )
            { 
                if (Type == StateType.FSM)
                {
                    if (FSMTransition.TryGetValue(src, dst, out var transition))
                    {
                        var dstState = GetState(dst);
                        if (dstState == null)
                        {
                            BeforeCheckTranstion(transition);
                        }
                        else
                        {
                            dstState.BeforeCheckTranstion(transition);
                        }

                        return transition.CheckCondition();

                    }
                }
                else
                {
                    if (ParallelTransition.TryGetValue(src, dst, out var transition))
                    {
                        var dstState = GetState(dst);
                        if(dstState == null)
                        {
                            BeforeCheckTranstion(transition);
                        }
                        else
                        {
                            dstState.BeforeCheckTranstion(transition);
                        }
                        return transition.CheckCondition();
                    }
                } 
                return false;
            }
            else
            {
                return false;
            }
        }
        public void DoTransitionTo(string dsc)
        {
            if (Type == StateType.FSM)
            {
                if (FSMTransition.TryGetValueByKey2(dsc, out var transitionList))
                {
                    foreach (var kvp in transitionList)
                    {
                        if (DoTransition(kvp.Key, dsc))
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                if (ParallelTransition.TryGetValueByKey2(dsc, out var transitionList))
                {
                    foreach (var kvp in transitionList)
                    {
                        DoTransition(kvp.Key, dsc);
                    }
                }
            }
        }
        public void DoTransitionFrom(string src)
        {
            if (Type == StateType.FSM)
            {
                if (FSMTransition.TryGetValueByKey1(src, out var transitionList))
                {
                    foreach (var kvp in transitionList)
                    {
                        if (DoTransition(src, kvp.Key))
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                if (ParallelTransition.TryGetValueByKey1(src, out var transitionList))
                {
                    foreach (var kvp in transitionList)
                    {
                        DoTransition(src, kvp.Key);
                    }
                }
            }
        }
       
        public bool DoTransition(string src, string dst)
        {
            if (Type == StateType.FSM)
            {
                if (CheckTransitionValid(src, dst))
                {
                    if (src == Name)
                    {
                        if (FSMTask == null)
                        {
                            var newState = GetFSMState(dst);
                            if (newState != null)
                            {
                                FSMTask = newState;
                                FSMTask.Enter();
                                if (Active)
                                {
                                    FSMTask.Activate();
                                }
                                return true;
                            }
                        }
                    }
                    else if (dst == Name)
                    {
                        var state = GetFSMState(src);
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
                        var srcState = GetFSMState(src);
                        var dstState = GetFSMState(dst);
                        if(CheckTask(src))
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
                    }
                }
            }
            else
            {
                if (CheckTransitionValid(src, dst))
                {
                    if (src == Name)
                    {
                        if (!ParallelTask.TryGetValue(dst, out var state))
                        {
                            state = GetParallelState(dst);
                            ParallelTask.Add(dst, state);
                            state.Enter();
                            if (Active) 
                            {
                                state.Activate();
                            }
                            return true;
                        }
                    }
                    else if (dst == Name)
                    {
                        if (ParallelTask.TryGetValue(src, out var state))
                        {
                            ParallelTask.Remove(src);
                            state.Exit();
                            if (state.Active)
                            {
                                state.InActivate();
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool DoAllTransition()
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
       
        public void BeforeCheckTranstion(ITransition transition)
        {
            OnBeforeCheckTranstion(transition);
        }
        public void Activate()
        {
            UnityEngine.Debug.Log("Activate : " + Name);
            Active = true;
            OnActivate();
            OnStateActivate?.Invoke(this);
            if(!DoAllTransition())
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
                        var state = GetParallelState(key);
                        state.Activate();
                    }
                }
            }
        }
        public void Enter()
        {
            Task = true;
            UnityEngine.Debug.Log("Enter : " +Name);
            OnEnter();
            OnStateEnter?.Invoke(this);
        }
        public void Update()
        {
            OnUpdate();
            OnStateUpdate?.Invoke(this);
            if (!DoAllTransition())
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
                        var state = GetParallelState(key);
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
            UnityEngine.Debug.Log("Exit : " + Name);
            Task = false; 
            OnExit();
            OnStateExit?.Invoke(this);
        }
        
        public void InActivate()
        {
            UnityEngine.Debug.Log("InActivate : " + Name);
            Active = false;
            OnInActivate();
            OnStateInActivate?.Invoke(this);
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
                    var state = GetParallelState(key);
                    state.InActivate();
                }
            }
        }
        public bool CheckTask(string name)
        {
            if (Type == StateType.FSM)
            {
                if(FSMTask == null)
                {
                    if (name == Name)
                    {
                        return true;
                    }
                }
                else
                {
                    if (name == FSMTask.Name)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (name == Name)
                {
                    return true;
                }
                if (ParallelTask.TryGetValue(name, out var _))
                {
                    return true;
                }
            }
            return false;
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
            if (e.ShouldActive && !Active)
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
        public virtual void OnBeforeCheckTranstion(ITransition transition)
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
                current = current.GetState(part);
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



        public Dictionary<string, IState> GetAllChild()
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