using System.Collections.Generic;
using UnityEngine;
namespace STC
{
    public interface IState
    {
        string Name { get;}
        IState ParentState { get; }
        IState GetChildState(string name);

        Dictionary<string, IState>.KeyCollection GetAllTaskState();
        Dictionary<string, IState>.KeyCollection GetAllChildState();
        string GetFullPath();
        void TaskInChlid(string name);
        void TaskOutChlid(string name);
        void TaskChlid(string inTaskState, string outTaskState);
        void SetParent(IState state);
        void Enter();
        void Exit();
        void Update();
    }
}