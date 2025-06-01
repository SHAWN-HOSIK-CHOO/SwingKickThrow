using System;
using UnityEngine;

namespace STC
{
    public interface ITransition 
    {
        string Src { get; } 
        string Dst { get; }
        ICondition Condition { get; }
        event Action<ITransition> ConditionValidSignal;
        bool CheckCondition();

    }
}