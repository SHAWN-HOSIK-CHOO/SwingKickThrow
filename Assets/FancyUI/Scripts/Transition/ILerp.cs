using UnityEngine;

public interface ILerp<T> 
{
    T Lerp(Transition<T> transition, float t);
}
