using FancyUI.MVVM.View.Transition;
using System.Collections.Generic;
using UnityEngine;

namespace FancyUI.Transition
{
    public class TransitionContainer : MonoTransition
    {
        public bool Override;
        public double OverrideDuration;
        public List<MonoBehaviour> TransitionList;
        private double _simulDuration; // Not Used
        public override double duration
        {
            get => Override ? OverrideDuration : CalMaxDuration();
            set => _simulDuration = value;
        }
        public override void Apply(double t)
        {
            foreach (var obj in TransitionList)
            {
                if (obj is MVVM.View.Transition.ITransition transition)
                {
                    transition.Apply(t);
                }
            }
        }

        double CalMaxDuration()
        {
            double MaxDuration = 0f;
            foreach (var obj in TransitionList)
            {
                if (obj is MVVM.View.Transition.ITransition transition)
                {
                    MaxDuration = MaxDuration > transition.duration ? MaxDuration : transition.duration;
                }
            }
            return MaxDuration;
        }

        public override void Initialize()
        {

            foreach (var obj in TransitionList)
            {
                if (obj is MVVM.View.Transition.ITransition transition)
                {
                    transition.Initialize();
                }
            }
        }
    }
}