using MVVM.Property;
using System;
using UnityEngine;

namespace STC
{
    public interface ICondition
    {
        string Name { get; }
        BoolProperty IsValid { get; }
        bool CheckValid();
        ICondition AddCondition(ICondition condition);
    }
}