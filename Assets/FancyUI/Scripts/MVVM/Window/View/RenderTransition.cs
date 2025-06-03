using FancyUI;
using FancyUI.MVVM.View.Transition;
using MVVM.View.Window;
using UnityEngine;

public class RenderTransition : MonoBehaviour
{
    public Window Owner;
    double StartTime;
    double CurrentTime;
    bool Transitioning;
    public MonoTransition transition;
    public void TransitionStart() => Owner.TransitionStart();
    public void EndTransition() => Owner.TransitionEnd();
    public void OnTransitionStart()
    {
        StartTime = Time.realtimeSinceStartupAsDouble;
        CurrentTime = StartTime;
        Transitioning = true;
        if(transition != null)
        {
            ResetTransition();
        }
        else
        {
            EndTransition();
        }

    }

    public void ResetTransition()
    {
        transition.Initialize();
        transition.Apply(0);
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
