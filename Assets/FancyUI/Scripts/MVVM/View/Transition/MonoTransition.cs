using MVVM.View.Transition;
using System;
using UnityEngine;
namespace FancyUI
{
    public class MonoTransition : MonoBehaviour, ITransition
    {
        [SerializeField]
        private double _duration = 0;
        public virtual double duration
        {
            get => _duration;
            set => _duration = value;
        }

        public event Action TransitionEndCallback;

        public virtual void Apply(double t)
        {

        }

        public virtual void Initialize()
        {

        }
    }
}