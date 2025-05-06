using System.Collections.Generic;
using UnityEngine;
namespace FSM
{
    public abstract class BsaeTanstition<StateEnum> : ITransition<StateEnum>
    {
        public abstract StateEnum _before { get;}
        public abstract StateEnum _after { get;}

        StateEnum ITransition<StateEnum>.before => _before;
        StateEnum ITransition<StateEnum>.after => _after;

        List<ICondition> _conditionList = new();
        void ITransition<StateEnum>.AddCondition(ICondition condition)
        {
            _conditionList.Add(condition);
        }

        bool ITransition<StateEnum>.CheckCondition()
        {
            bool conditoin = true;
            foreach (ICondition condition in _conditionList)
            {
                conditoin &= condition.Check();
            }
            return conditoin;
        }
    }
}