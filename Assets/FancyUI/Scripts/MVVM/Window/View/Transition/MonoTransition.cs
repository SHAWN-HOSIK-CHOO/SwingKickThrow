using System;
using MVVM.View.Transition;
using UnityEngine;

namespace FancyUI.Scripts.MVVM.View.Transition
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

        public virtual void Apply(double t)
        {

        }

        public virtual void Initialize()
        {

        }
    }
}