using System;
using System.Collections;
using GameEvents;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace CameraUtils
{
    public class PlayerEffectCameraController : MonoBehaviour
    {
        [SerializeField]
        private Camera _playerEffectCamera;
        //public  float  pauseDuration = 0.4f;

        [Header("Time Slow Settings")]
        public float targetTimeScale = 0.05f;
        public float timeFadeOut = 0.1f;
        public float timeHold    = 0.3f;
        public float timeFadeIn  = 0.1f;
        
        [Header("Camera Zoom Settings")]
        public CinemachineCamera virtualCam;
        public CinemachineCamera zoomCam;
        //public float             zoomFOV      = 50f;
        public float             zoomDuration = 0.1f;
        public float             zoomHold     = 0.3f;
        public float             defaultFOV   = 60f;
        
        private Coroutine _zoomRoutine;

        [Header("Camera Shake , Set up when network player is spawned")] 
        public CinemachineImpulseSource cinemachineImpulseSource;

        private void OnEnable()
        {
            GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_CRASH,PlayHitFeedback);
            //GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_CRASH_END,LowerPriority);
        }

        private void OnDisable()
        {
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_CRASH,PlayHitFeedback);
            //GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_CRASH_END,LowerPriority);
        }

        private void HigherPriority()
        {
            _playerEffectCamera.depth = 0;
        }

        private void LowerPriority()
        {
            _playerEffectCamera.depth = -2;
        }

        private void PlayHitFeedback()
        {
            StartCoroutine(HitFeedbackRoutine());
        }

        private IEnumerator HitFeedbackRoutine()
        {
            HigherPriority();

            if (cinemachineImpulseSource != null)
            {
                Debug.Log("Generating impulse");
                cinemachineImpulseSource.GenerateImpulse();
            }
            
            zoomCam.Priority    = 11;
            virtualCam.Priority = 10;
            // 카메라 줌 인 시작
            if (_zoomRoutine != null)
                StopCoroutine(_zoomRoutine);
            _zoomRoutine = StartCoroutine(ZoomCoroutine(defaultFOV, zoomDuration));

            // ⬇️ 시간 서서히 느려지기
            float t = 0f;
            while (t < timeFadeOut)
            {
                t              += Time.unscaledDeltaTime;
                Time.timeScale =  Mathf.Lerp(1f, targetTimeScale, t / timeFadeOut);
                yield return null;
            }
            Time.timeScale = targetTimeScale;

            yield return new WaitForSecondsRealtime(timeHold);

            // ⬆️ 시간 회복
            t = 0f;
            while (t < timeFadeIn)
            {
                t              += Time.unscaledDeltaTime;
                Time.timeScale =  Mathf.Lerp(targetTimeScale, 1f, t / timeFadeIn);
                yield return null;
            }
            Time.timeScale = 1f;

            // 카메라 줌 아웃 복구
            _zoomRoutine = StartCoroutine(ZoomCoroutine(defaultFOV, zoomDuration));
            yield return new WaitForSecondsRealtime(zoomHold);

            zoomCam.Priority    = 10;
            virtualCam.Priority = 11;
            
            LowerPriority();
        }

        private IEnumerator ZoomCoroutine(float targetFOV, float duration)
        {
            float startFOV = virtualCam.Lens.FieldOfView;
            float t        = 0f;

            while (t < duration)
            {
                t                           += Time.unscaledDeltaTime;
                virtualCam.Lens.FieldOfView =  Mathf.Lerp(startFOV, targetFOV, t / duration);
                yield return null;
            }

            virtualCam.Lens.FieldOfView = targetFOV;
        }
    }
}
