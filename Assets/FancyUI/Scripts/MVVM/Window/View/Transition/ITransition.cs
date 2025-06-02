using FancyUI;
using System;
using UnityEngine;

namespace MVVM.View.Transition
{
    public interface ITransition
    {
        double duration { get; }
        void Initialize();
        void Apply(double t);
    }
}