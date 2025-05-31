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

        event Action<IState> OnStateActivate;
        event Action<IState> OnStateEnter;
        event Action<IState> OnStateUpdate;
        event Action<IState> OnStateExit;
        event Action<IState> OnStateInActivate;
        void Handle(Event.IStateEvent e);



        void Activate();
        void BeforeCheckTranstion(ITransition transition);
        void Enter();
        void Update();
        void Exit();

        void InActivate();
        // Utils
        Dictionary<string, IState> GetAllChild();
        string GetFSMTask();
        bool CheckTask(string name);
        string GetFullPath();
        IState GetState(string name);
        IState FindChlid(string relativePath);
        
        bool DoTransition(string src, string dst);
        void DoTransitionFrom(string src);
        void DoTransitionTo(string dsc);

        bool DoAllTransition();
        bool CheckTransitionValid(string src, string dst);
        void SetParent(IState state);
        void AddChild(IState state);
        void AddTransition(ITransition condition);
        ITransition GetTransition(string src, string dst);
        void RemoveTransition(string src, string dst);
        void RemoveChild(IState state);
    }
}