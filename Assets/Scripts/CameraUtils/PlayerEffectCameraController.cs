using System;
using GameEvents;
using UnityEngine;

namespace CameraUtils
{
    public class PlayerEffectCameraController : MonoBehaviour
    {
        private Camera _playerEffectCamera;

        private void Awake()
        {
            _playerEffectCamera = this.GetComponent<Camera>();
        }

        private void OnEnable()
        {
            GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_CRASH,HigherPriority);
            GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_CRASH_END,LowerPriority);
        }

        private void OnDisable()
        {
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_CRASH,HigherPriority);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_CRASH_END,LowerPriority);
        }

        private void HigherPriority()
        {
            _playerEffectCamera.depth = 0;
        }

        private void LowerPriority()
        {
            _playerEffectCamera.depth = -2;
        }
    }
}
