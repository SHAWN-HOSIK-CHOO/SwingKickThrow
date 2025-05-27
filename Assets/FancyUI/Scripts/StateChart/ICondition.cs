using MVVM.Property;
using System;
using UnityEngine;

namespace STC
{
    public interface ICondition
    {
        Property<bool> IsActive { get; }

        event Action Dirty;

        bool CheckCondition();
        void AddCondition(ICondition condition);
        void RemoveCondition(ICondition condition);
        
        void RaiseDirty();
    }
}