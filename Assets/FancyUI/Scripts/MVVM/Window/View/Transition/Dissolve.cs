using UnityEngine;
namespace FancyUI.Transition.Effect
{
    public class Dessolve : ImageTransition
    {
        [SerializeField] float noiseScale;

        public override void Initialize()
        {
            base.Initialize();
            obj.material.SetFloat("_NoiseScale", noiseScale);

        }
    }
}