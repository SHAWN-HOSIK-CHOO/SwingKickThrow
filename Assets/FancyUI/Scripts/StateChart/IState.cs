using System;
using System.Collections.Generic;
using static STC.State;
namespace STC
{
    public interface IState
    {
        string Name { get;}
        IState ParentState { get; }
        StateType Type { get; }
        event Action<IState> OnStateEnter;
        event Action<IState> OnStateUpdate;
        event Action<IState> OnStateExit;
        void Handle(Event.IStateEvent e);
        void Enter();
        void Update();
        void Exit();

        // Utils
        Dictionary<string, IState> GetAllChild();
        bool CheckTask(string name);
        string GetFullPath();
        IState GetState(string name);
        IState FindChlid(string relativePath);
        bool DoTransition(string src, string dst);
        void DoTransitionFrom(string src);
        void DoTransitionTo(string dsc);
        bool CheckTransition(string src, string dst);
        void SetParent(IState state);
        void AddChild(IState state);
        void AddTransition(ITransition condition);
        ITransition GetTransition(string src, string dst);
        void RemoveTransition(string src, string dst);
        void RemoveChild(IState state);
    }
}