using System;
using System.Collections;
using UnityEngine;
using GameEvents;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace CharacterAttributes.PlayerUI
{
    public class PlayerUIController : MonoBehaviour {
        public GameObject pfBlackoutUI;
        public float      pauseDuration = 0.4f;
        
        private GameObject _blackoutUI;

        public  UniversalRendererData     rendererData;
        private ScriptableRendererFeature _targetFeature;

        private bool _isOwner = false;
        
        public void Initialize(CharacterControllerNetwork characterControllerNetwork)
        {
            //Debug.Log($"Initialized : {characterControllerNetwork.IsOwner}");
            _isOwner = characterControllerNetwork.IsOwner;
            
            foreach (var feature in rendererData.rendererFeatures)
            {
                if (feature.name == "FullScreenPassRendererFeature") 
                {
                    _targetFeature = feature;
                    break;
                }
            }
            
            if(_targetFeature == null)
                Debug.Log("There is no feature as such : Full Screen Pass Renderer Feature");
            else
            {
                _targetFeature.SetActive(false);
            }

            if (_isOwner)
            {
                GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_DASH,     ActivateSpeedLine);
                GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_DASH_END, InActivateSpeedLine);
            }
        }
        
        private void OnDisable()
        {
            if (!_isOwner)
            {
                return;
            }
            
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH,     ActivateSpeedLine);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH_END, InActivateSpeedLine);
        }

        private void ActivateSpeedLine()
        {
            _targetFeature.SetActive(true);
        }

        private void InActivateSpeedLine()
        {
            _targetFeature.SetActive(false);
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
