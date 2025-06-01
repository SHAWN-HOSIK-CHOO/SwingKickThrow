using System;
using UnityEngine;

namespace STC
{
    public class Transition : ITransition
    {
        public string Src { get; private set; }
        public string Dst { get; private set; }

        public event Action<ITransition> ConditionValidSignal;
        public ICondition Condition { get; private set; }
        public Transition(string src, string dst)
        {
            Src = src;
            Dst = dst;
            Condition = new Condition(nameof(Transition),true);
            Condition.IsValid.Subscribe(Signal);
        }
        void Signal(bool val)
        {
            if (val)
            {
                ConditionValidSignal?.Invoke(this);            
            }
        }
        public bool CheckCondition()
        {
            Condition.IsValid.SetSilently(Condition.CheckValid());
            return Condition.IsValid.Value;
        }
    }
}