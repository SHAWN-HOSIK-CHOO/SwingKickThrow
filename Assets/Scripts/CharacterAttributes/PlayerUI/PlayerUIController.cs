using System;
using System.Collections;
using UnityEngine;
using GameEvents;
using UnityEngine.Serialization;

namespace CharacterAttributes.PlayerUI
{
    public class PlayerUIController : MonoBehaviour
    {
        public GameObject pfBlackoutUI;
        public float      pauseDuration = 1f;

        private GameObject _blackoutUI;
        
        private void OnEnable()
        {
            GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_CRASH,CoStartBlackoutSequence);
            // 씬에 있는 Canvas 찾기
            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas != null && _blackoutUI == null)
            {
                _blackoutUI = Instantiate(pfBlackoutUI, canvas.transform);
                _blackoutUI.SetActive(false);
            }
        }

        private void OnDisable()
        {
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_CRASH,CoStartBlackoutSequence);
        }

        private void CoStartBlackoutSequence()
        {
            StartCoroutine(BlackoutCoroutine());
        }

        private IEnumerator BlackoutCoroutine()
        {
            _blackoutUI.SetActive(true);
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(pauseDuration);
            Time.timeScale = 1f;
            _blackoutUI.SetActive(false);
        }
    }
}
