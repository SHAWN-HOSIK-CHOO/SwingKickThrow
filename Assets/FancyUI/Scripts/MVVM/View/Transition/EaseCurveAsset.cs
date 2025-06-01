using UnityEngine;

namespace FancyUI
{
    [CreateAssetMenu(menuName = "Easing/EaseCurveAsset", fileName = "NewEaseCurve")]
    public class EaseCurveAsset : ScriptableObject
    {
        public enum EaseType
        {
            Linear,
            EaseInSine,
            EaseOutSine,
            EaseInOutSine,
            EaseInQuad,
            EaseOutQuad,
            EaseInOutQuad,
            EaseInCubic,
            EaseOutCubic,
            EaseInOutCubic,
            EaseInQuart,
            EaseOutQuart,
            EaseInOutQuart,
            Custom,  // 추가된 커스텀 타입
        }

        public EaseType easeType = EaseType.EaseInSine;
        public AnimationCurve curve;
        public int resolution = 32;

        [ContextMenu("Generate Curve")]
        public void GenerateCurve()
        {
            if (easeType == EaseType.Custom)
            {
                // 커스텀 타입이면 생성하지 않고 그냥 리턴
                Debug.Log("Custom ease type selected, skipping curve generation.");
                return;
            }

            Keyframe[] keys = new Keyframe[resolution];
            for (int i = 0; i < resolution; i++)
            {
                float t = i / (float)(resolution - 1);
                float value = EvaluateFunction(t);
                keys[i] = new Keyframe(t, value);
            }
            curve = new AnimationCurve(keys);
        }

        private float EvaluateFunction(float t)
        {
            // Refers : https://easings.net/ 
            switch (easeType)
            {
                case EaseType.Linear:
                    return t;
                case EaseType.EaseInSine:
                    return 1f - Mathf.Cos((t * Mathf.PI) / 2f);
                case EaseType.EaseOutSine:
                    return Mathf.Sin((t * Mathf.PI) / 2f);
                case EaseType.EaseInOutSine:
                    return -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;
                case EaseType.EaseInQuad:
                    return t * t;
                case EaseType.EaseOutQuad:
                    return 1 - (1 - t) * (1 - t);
                case EaseType.EaseInOutQuad:
                    return t < 0.5 ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
                case EaseType.EaseInCubic:
                    return t * t * t;
                case EaseType.EaseOutCubic:
                    return 1f - Mathf.Pow(1f - t, 3);
                case EaseType.EaseInOutCubic:
                    return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3) / 2f;
                case EaseType.EaseInQuart:
                    return t * t * t * t;
                case EaseType.EaseOutQuart:
                    return 1 - Mathf.Pow(1 - t, 4);
                case EaseType.EaseInOutQuart:
                    return t < 0.5 ? 8 * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 4) / 2;
                case EaseType.Custom:
                    // 커스텀은 여기서 처리하지 않음
                    return t;
                default:
                    return t;
            }
        }
    }
}
