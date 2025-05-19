using UnityEngine;
namespace FSM
{
    public interface ITransition<StateEnum>
    {
        StateEnum before { get; } 
        StateEnum after { get; }
        bool CheckCondition();
        void AddCondition(ICondition condition);
    }
}