using System;
using UnityEngine;

namespace STC
{
    public class Transition : ITransition
    {
        public string Src { get; private set; }
        public string Dst { get; private set; }

        public event Action<ITransition> Dirty;
        public ICondition Condition { get; private set; }
        public Transition(string src, string dst)
        {
            Src = src;
            Dst = dst;
            Condition = new Condition(nameof(Transition),true);
            Condition.Dirty += ConditionDirty;
        }
        public void AddCondition(ICondition condition)
        {
            condition.Dirty += ConditionDirty;
            Condition.AddCondition(condition);
        }
        void ConditionDirty()
        {
            Dirty?.Invoke(this);
        }
        public bool CheckCondition()
        {
            return Condition.CheckCondition();
        }
    }
}