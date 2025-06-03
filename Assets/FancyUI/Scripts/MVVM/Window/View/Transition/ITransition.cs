using FancyUI;
using System;
using UnityEngine;

namespace FancyUI.MVVM.View.Transition
{
    public interface ITransition
    {
        double duration { get; set; }
        void Initialize();
        void Apply(double t);
    }
}