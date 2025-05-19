using FancyUI;
using System.Collections;
using UnityEngine;
namespace FancyUI
{
    public class RectTransformTransition : MonoBehaviour, ITransition , ILerp<RectTransform>
    {
        [SerializeField] RectTransform from;
        [SerializeField] RectTransform to;
        [SerializeField] RectTransform obj;
        [SerializeField] EaseCurveAsset easeCurveAsset;
        [SerializeField] float duration = 0.5f;
        [Header("Rect Lerp")]
        [SerializeField] internal bool usePosition;
        [SerializeField] internal bool useRotation;
        [SerializeField] internal bool useScale;
        Transition<RectTransform> transition => new Transition<RectTransform>(from, to, obj, this, easeCurveAsset);

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
        public void Initialize()
        {
            transition.obj.position = transition.from.position;
            transition.obj.rotation = transition.from.rotation;
            transition.obj.sizeDelta = transition.from.sizeDelta;
        }
        private IEnumerator DoTransition()
        {
            float elapsed = 0f;
            while (transition.Apply(duration,elapsed))
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        public bool Apply(float duration, float elapsed) => transition.Apply(duration, elapsed);

        public RectTransform Lerp(Transition<RectTransform> transition, float t)
        {
            if (transition.from == null || transition.to == null || transition.obj == null)
            {
                Debug.LogWarning("Panels are not correctly assigned!");
                return transition.obj;
            }

            float curvedT = transition.curve.curve.Evaluate(t);

            if (usePosition)
            {
                Vector3 fromWorldPos = transition.from.position;
                Vector3 toWorldPos = transition.to.position;
                transition.obj.position = Vector3.Lerp(fromWorldPos, toWorldPos, curvedT);
            }

            if (useRotation)
            {
                Quaternion fromRot = transition.from.rotation;
                Quaternion toRot = transition.to.rotation;
                transition.obj.rotation = Quaternion.Slerp(fromRot, toRot, curvedT);
            }

            if (useScale)
            {
                Vector2 fromSize = transition.from.sizeDelta;
                Vector2 toSize = transition.to.sizeDelta;
                transition.obj.sizeDelta = Vector2.Lerp(fromSize, toSize, curvedT);
            }
            return transition.obj;
        }
    }
}