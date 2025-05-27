using System;
using UnityEngine;

namespace STC
{
    public interface ITransition 
    {
        string Src { get; } 
        string Dst { get; }
        event Action<ITransition> Dirty;
        void AddCondition(ICondition condition);
        bool CheckCondition();

    }
}