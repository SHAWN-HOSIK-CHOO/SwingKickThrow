using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace STC
{
    public interface IState
    {
        string Name { get;}
        IState ParentState { get; }
        event Action<IState> OnStateEnter;
        event Action<IState> OnStateSetUp;
        event Action<IState> OnStateUpdate;
        event Action<IState> OnStateExit;
        event Action<IState> OnStateFinish;
        IState GetChildState(string name);
        List<string> GetTaskOutMap();
        List<string> GetAllTaskState();
        List<string> GetAllChildState();
        string GetFullPath();
        IState FindChlid(string relativePath);
        void TaskInChlid(string name);
        void TaskOutChlid(string name);
        void TaskChlid(string inTaskState, string outTaskState);
        void SetParent(IState state);
        void AddChild(IState state);
        void RemoveChild(IState state);
        void AddTaskOutMap(IState state);
        void RemoveTaskOutMap(IState state);
        void SetUp();
        void Handle(global::STC.Event.IStateEvent e);
        void Enter();
        void Update();
        void Exit();
        void Finish();
    
        
    
    }
}