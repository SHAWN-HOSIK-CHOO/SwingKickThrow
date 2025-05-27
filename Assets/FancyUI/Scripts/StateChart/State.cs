using STC.Event;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public event Action<IState> OnStateEnter;
        public event Action<IState> OnStateUpdate;
        public event Action<IState> OnStateExit;

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
        public bool CheckTransition(string src, string dst)
        {
            if (Type == StateType.FSM)
            {
                if (FSMTransition.TryGetValue(src, dst, out var transition))
                {
                    return transition.CheckCondition();
                }
            }
            else
            {
                if (ParallelTransition.TryGetValue(src, dst, out var transition))
                {
                    return transition.CheckCondition();
                }
            }
            return false;
        }
        public bool DoTransition(string src, string dst)
        {
            if (Type == StateType.FSM)
            {
                if (CheckTransition(src, dst))
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
                            FSMTask = dstState;
                            FSMTask.Enter();
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (CheckTransition(src, dst))
                {
                    if (src == Name)
                    {
                        if (!ParallelTask.TryGetValue(dst, out var state))
                        {
                            state = GetParallelState(dst);
                            ParallelTask.Add(dst, state);
                            state.Enter();
                            return true;
                        }
                    }
                    else if (dst == Name)
                    {
                        if (ParallelTask.TryGetValue(src, out var state))
                        {
                            ParallelTask.Remove(src);
                            state.Exit();
                            return true;
                        }
                    }
                }
            }
            return false;
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
        public void Enter()
        {
            OnEnter();
            OnStateEnter?.Invoke(this);
            DoTransitionFrom(Name);
        }
        public void Update()
        {
            OnUpdate();
            OnStateUpdate?.Invoke(this);

            if (Type == StateType.FSM)
            {
                if (FSMTask != null)
                {
                    FSMTask.Update();
                }
            }
            else
            {
                foreach (var kvp in ParallelTask)
                {
                    kvp.Value.Update();
                }
            }
        }
        public void Exit()
        {
            if (Type == StateType.FSM)
            {
                if (FSMTask != null)
                {
                    var state = FSMTask;
                    FSMTask = null;
                    state.Exit();
                }
            }
            else
            {
                foreach (var key in ParallelTask.Keys.ToList())
                {
                    var state = GetParallelState(key);
                    ParallelTask.Remove(key);
                    state.Exit();
                }
            }
            OnExit();
            OnStateExit?.Invoke(this);
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
                if (ParallelTask.TryGetValue(name, out var _))
                {
                    return true;
                }
            }
            return false;
        }
        public void Handle(IStateEvent e)
        {
            bool checkValid = false;
            if (ParentState != null)
            {
                if (ParentState.Type == StateType.FSM)
                {
                    if (ParentState.CheckTask(Name) && !e.Consumed.isConsumed)
                    {
                        checkValid = true;
                    }
                }
                else
                {
                    if (ParentState.CheckTask(Name))
                    {
                        checkValid = true;
                    }
                }
            }

            if (checkValid)
            {
                if (Type == StateType.FSM)
                {
                    if (FSMTask != null)
                    {
                        FSMTask.Handle(e);
                    }

                    if (!e.Consumed.isConsumed)
                    {
                        OnHandle(e);
                    }
                }
                else
                {
                    e.Consumed = new ConsumeEvent();
                    foreach (var state in ParallelTask)
                    {
                        state.Value.Handle(e);
                    }
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
        }
        public void AddChild(IState state)
        {
            if (Type == StateType.FSM)
            {
                FSMState.Add(state.Name, state);
            }
            else
            {
                ParallelState.Add(state.Name, state);
            }
        }
        public void RemoveChild(IState state)
        {
            if (state.Type == StateType.FSM)
            {
                FSMState.Remove(state.Name);
            }
            else
            {
                ParallelState.Remove(state.Name);
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
            transition.Dirty += TransitionDirty;
        }

        void TransitionDirty(ITransition transition)
        {
            var dst = GetState(transition.Dst);

            StateChart.Instance.PublishEvent(new TaskEvent(dst));
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