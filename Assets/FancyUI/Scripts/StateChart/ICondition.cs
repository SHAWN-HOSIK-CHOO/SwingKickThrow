using MVVM.Property;
using System;
using UnityEngine;

namespace STC
{
    public interface ICondition
    {
        string Name { get; }
        BoolProperty IsActive { get; }
        BoolProperty IsValid { get; }
        void CheckValid(ICondition except);
        ICondition AddCondition(ICondition condition);
    }
}