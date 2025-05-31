using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using static STC.State;
namespace STC
{
    public interface IState
    {
        string Name { get;}
        bool Task { get;  }
        bool Active { get; }
        IState ParentState { get; }
        StateType Type { get; }
        void Handle(Event.IStateEvent e);
        void Activate();
        void CheckTransitionBarrier(string src);
        void Enter();
        void Update();
        void Exit();
        void InActivate();
        // Utils
        Dictionary<string, IState> GetAllChildState();
        bool IsTask(string name);
        string GetFullPath();
        IState GetChildState(string name);
        IState FindChlid(string relativePath);
        bool DoTransition();
        void SetParent(IState state);
        void AddChild(IState state);
        void AddTransition(ITransition condition);
        ITransition GetTransition(string src, string dst);
        void RemoveTransition(string src, string dst);
        void RemoveChild(IState state);
    }
}