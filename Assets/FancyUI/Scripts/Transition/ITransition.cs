using UnityEngine;


public interface ITransition
{
    float duration { get; }
    void Initialize();
    bool Apply(float duration,float elapsed);
    void Play();
}
