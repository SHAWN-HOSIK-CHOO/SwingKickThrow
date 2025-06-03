using FancyUI;
using FancyUI.MVVM.View.Transition;
using System;
using System.Collections;
using UnityEngine;
namespace FancyUI
{
    public class RectTransformTransition : MonoTransition
    {
        [SerializeField] RectTransform from;
        [SerializeField] RectTransform to;
        [SerializeField] RectTransform obj;
        [SerializeField] EaseCurveAsset easeCurveAsset;

        [Header("Rect Lerp")]
        [SerializeField] internal bool usePosition;
        [SerializeField] internal bool useRotation;
        [SerializeField] internal bool useScale;


        public override void Initialize()
        {
            // 부모가 같은 경우, 로컬 좌표 기준으로 초기화
            obj.anchoredPosition = from.anchoredPosition;
            obj.localRotation = from.localRotation;
            obj.sizeDelta = from.sizeDelta;
            obj.pivot = from.pivot;
            obj.anchorMin = from.anchorMin;
            obj.anchorMax = from.anchorMax;
        }


        public override void Apply(double t)
        {
            if (from == null || to == null || obj == null)
            {
                Debug.LogWarning("Panels are not correctly assigned!");
                return;
            }

            float curvedT = easeCurveAsset.curve.Evaluate((float)t);

            if (usePosition)
            {
                Vector2 fromPos = from.anchoredPosition;
                Vector2 toPos = to.anchoredPosition;
                obj.anchoredPosition = Vector2.Lerp(fromPos, toPos, curvedT);
            }

            if (useRotation)
            {
                Quaternion fromRot = from.localRotation;
                Quaternion toRot = to.localRotation;
                obj.localRotation = Quaternion.Slerp(fromRot, toRot, curvedT);
            }

            if (useScale)
            {
                // sizeDelta 보간
                Vector2 fromSize = from.sizeDelta;
                Vector2 toSize = to.sizeDelta;
                obj.sizeDelta = Vector2.Lerp(fromSize, toSize, curvedT);

                // pivot 보간 (필요하면)
                Vector2 fromPivot = from.pivot;
                Vector2 toPivot = to.pivot;
                obj.pivot = Vector2.Lerp(fromPivot, toPivot, curvedT);

                // anchor 보간 (필요하면)
                Vector2 fromAnchorMin = from.anchorMin;
                Vector2 toAnchorMin = to.anchorMin;
                obj.anchorMin = Vector2.Lerp(fromAnchorMin, toAnchorMin, curvedT);

                Vector2 fromAnchorMax = from.anchorMax;
                Vector2 toAnchorMax = to.anchorMax;
                obj.anchorMax = Vector2.Lerp(fromAnchorMax, toAnchorMax, curvedT);
            }
        }
    }
}