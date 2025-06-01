using FancyUI;
using MVVM.View.Transition;
using MVVM.View.Window;
using UnityEngine;

public class RenderTransition : MonoBehaviour
{
    public Window Owner;

    double StartTime;
    double CurrentTime;
    bool Transitioning;
    public MonoTransition transition;


    public void Awake()
    {
        if(transition != null)
        {
            transition.TransitionEndCallback += EndTransition;
        }
    }
    public void StartTransition() => Owner.Binder.ViewBinding.TransitionStart();
    public void EndTransition() => Owner.Binder.ViewBinding.TransitionEnd();
    public void OnTransitionStart()
    {
        if(transition == null)
        {
            EndTransition();
        }
        StartTime = Time.realtimeSinceStartupAsDouble;
        CurrentTime = StartTime;
        Transitioning = true;
        transition.Initialize();
    }
    public void OnTransitionEnd()
    {
        StartTime = Time.realtimeSinceStartupAsDouble;
        CurrentTime = StartTime;
        Transitioning = false;
    }

    public void LateUpdate()
    {
        if(Transitioning)
        {
            CurrentTime = Time.realtimeSinceStartupAsDouble;
            if(transition != null)
            {
                var elapsed = CurrentTime - StartTime;
                transition.Apply(Utils.Math.Math.Normalize(transition.duration, elapsed));
                if (elapsed >= transition.duration)
                {
                    EndTransition();
                }
            }
        }
    }
}
