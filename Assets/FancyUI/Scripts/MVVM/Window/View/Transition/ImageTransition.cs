
using FancyUI.MVVM.View.Transition;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace FancyUI.Transition.Effect
{
    public class ImageTransition : MonoTransition
    {
        [SerializeField] protected Sprite from;
        [SerializeField] protected Sprite to;
        [SerializeField] protected Image obj;
        [SerializeField] protected EaseCurveAsset easeCurveAsset;

        [Header("Sprite Lerp")]
        [SerializeField] protected Material blendMaterial;

        public override void Initialize()
        {
            obj.material = Instantiate(blendMaterial);
            obj.material.SetTexture("_FromTex", from.texture);
            obj.material.SetTexture("_ToTex", to.texture);
            obj.material.SetFloat("_Blend", 0f);
        }

        public override void Apply(double t)
        {
            float curvedT = easeCurveAsset.curve.Evaluate((float)t);
            obj.material.SetFloat("_Blend", curvedT);
        }
    }
}