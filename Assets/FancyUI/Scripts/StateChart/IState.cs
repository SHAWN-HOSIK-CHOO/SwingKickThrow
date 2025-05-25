using System.Collections.Generic;
using UnityEngine;
namespace STC
{
    public interface IState
    {
        string Name { get;}
        IState ParentState { get; }
        IState currentState { get; }
        IState GetChildState(string name);
        Dictionary<string, IState>.KeyCollection GetAllChildState();
        void DetachParent();
        string GetFullPath();
        void Enter();
        void Exit();
        void Update();
    }
}