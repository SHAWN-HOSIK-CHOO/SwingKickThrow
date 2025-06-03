using MVVM.View.Transition;
using System;
using System.Collections;
using FancyUI.Scripts.MVVM.View.Transition;
using UnityEngine;
using UnityEngine.UI;
namespace FancyUI
{
    public class ImageTransition : MonoTransition
    {
        [SerializeField] Sprite from;
        [SerializeField] Sprite to;
        [SerializeField] Image obj;
        [SerializeField] EaseCurveAsset easeCurveAsset;

        [Header("Sprite Lerp")]
        [SerializeField] Material blendMaterial;
        [SerializeField] float noiseScale;
        private MaterialPropertyBlock _propBlock;

        public override void Initialize()
        {
            obj.material = Instantiate(blendMaterial);
            obj.material.SetTexture("_FromTex", from.texture);
            obj.material.SetTexture("_ToTex", to.texture);
            obj.material.SetFloat("_NoiseScale", noiseScale);
            obj.material.SetFloat("_Blend", 0f);
        }

        public override void Apply(double t)
        {
            float curvedT = easeCurveAsset.curve.Evaluate((float)t);
            obj.material.SetFloat("_Blend", curvedT);
        }
    }
}