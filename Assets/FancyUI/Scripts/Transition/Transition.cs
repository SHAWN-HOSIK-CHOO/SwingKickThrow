using FancyUI;
using UnityEngine;


public struct Transition<T> 
{
    public T from;
    public T to;
    public T obj;
    public EaseCurveAsset curve;
    public ILerp<T> lerp;

    public Transition(T from, T to, T obj, ILerp<T> lerp, EaseCurveAsset curve )
    {
        this.from = from;
        this.to = to;
        this.obj = obj;
        this.lerp = lerp;
        this.curve = curve;
    }

    public float Normalize(float duration, float elapsed)
    {
        float normalizedT = Mathf.Clamp01((duration == 0) ? 1 : elapsed / duration);
        return normalizedT;
    }

    public bool Apply(float duration, float elapsed)
    {
        if (curve == null) return false;
        var normalizedT = Normalize(duration, elapsed);
        float curvedT = curve.curve.Evaluate(normalizedT);
        obj = lerp.Lerp(this, curvedT);
        return (normalizedT != 1);
    }
}

