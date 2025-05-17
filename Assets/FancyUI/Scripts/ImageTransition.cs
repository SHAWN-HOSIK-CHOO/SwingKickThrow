using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace FancyUI
{
    public class ImageTransition : MonoBehaviour, ITransition, ILerp<Image>
    {
        [SerializeField] Image from;
        [SerializeField] Image to;
        [SerializeField] Image obj;
        [SerializeField] EaseCurveAsset easeCurveAsset;
        [SerializeField] float duration = 0.5f;

        [Header("Sprite Lerp")]
        [SerializeField] Material blendMaterial;
        [SerializeField] float noiseScale;
        Transition<Image> transition => new Transition<Image>(from, to, obj, this, easeCurveAsset);
        private MaterialPropertyBlock _propBlock;

        float ITransition.duration => duration;

        Coroutine transitionCoroutine;
        public void Play()
        {
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            transitionCoroutine = StartCoroutine(DoTransition());
        }

        private IEnumerator DoTransition()
        {
            float elapsed = 0f;
            while (transition.Apply(duration, elapsed))
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        public void Initialize()
        {
            obj.material = Instantiate(blendMaterial);
            obj.material.SetTexture("_FromTex", from.sprite.texture);
            obj.material.SetTexture("_ToTex", to.sprite.texture);
            obj.material.SetFloat("_NoiseScale", noiseScale);
            obj.material.SetFloat("_Blend", 0f);
        }

        public bool Apply(float duration, float elapsed) => transition.Apply(duration, elapsed);

        public Image Lerp(Transition<Image> transition, float t)
        {
            if (transition.from == null || transition.to == null || transition.obj == null)
            {
                Debug.LogWarning("Panels are not correctly assigned!");
                return transition.obj;
            }

            float curvedT = transition.curve.curve.Evaluate(t);
            obj.material.SetFloat("_Blend", curvedT);
            return transition.obj;
        }
    }
}