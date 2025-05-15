using System;
using System.Collections;
using UnityEngine;
using GameEvents;
using GameFlow;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Unity.Netcode;
using CharacterAttributes;

namespace CharacterAttributes.PlayerUI
{
    public class PlayerUIController : MonoBehaviour
    {
        public UniversalRendererData rendererData;
        private ScriptableRendererFeature _targetFeature;

        private CharacterControllerNetwork _Owner;
        private bool _isOwner = false;

        private CharacterStatusNetwork _enemyStatus;
        
        #region TagBallStatus

        public TMP_Text tagBallCountText;
        public void SetTagBallCount(int prevVal, int newVal)
        {
            tagBallCountText.text = newVal.ToString();
        }

        #endregion

        #region ScoreStatus

        public TMP_Text playerScoreText;
        public TMP_Text enemyScoreText;

        public void SetPlayerScore(int prevVal, int newVal)
        {
            playerScoreText.text = newVal.ToString();
        }

        public void SetEnemyScore(int prevVal, int newVal)
        {
            //Debug.Log($"[EnemyScore] prev: {prevVal}, new: {newVal}");
            
            if (enemyScoreText != null)
                enemyScoreText.text = newVal.ToString();
            else
                Debug.LogWarning("enemyScoreText is null! Check GameSceneManager setup.");
        }

        #endregion

        public void Initialize(CharacterControllerNetwork characterControllerNetwork)
        {
            _isOwner = characterControllerNetwork.IsOwner;

            foreach (var feature in rendererData.rendererFeatures)
            {
                if (feature.name == "FullScreenPassRendererFeature")
                {
                    _targetFeature = feature;
                    break;
                }
            }

            if (_targetFeature == null)
                Debug.Log("There is no feature named 'FullScreenPassRendererFeature'");
            else
                _targetFeature.SetActive(false);

            if (_isOwner)
            {
                _Owner = characterControllerNetwork;
                GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_DASH, ActivateSpeedLine);
                GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_DASH_END, InActivateSpeedLine);

                tagBallCountText = GameSceneManager.Instance.tagBallCountText;
                playerScoreText = GameSceneManager.Instance.playerScoreText;
                enemyScoreText = GameSceneManager.Instance.enemyScoreText;

                if (enemyScoreText == null)
                    Debug.LogWarning("enemyScoreText is null! Check GameSceneManager setup.");
            }
        }

        private void Start()
        {
            if (_isOwner)
            {
                SetTagBallCount(0, _Owner.characterStatusNetwork.tagBalls.Value);
                _Owner.characterStatusNetwork.tagBalls.OnValueChanged   -= SetTagBallCount;
                _Owner.characterStatusNetwork.scoreBalls.OnValueChanged -= SetPlayerScore;
                _Owner.characterStatusNetwork.tagBalls.OnValueChanged   += SetTagBallCount;
                _Owner.characterStatusNetwork.scoreBalls.OnValueChanged += SetPlayerScore;

                // 적 점수 업데이트용 구독 시작
                StartCoroutine(SubscribeToEnemyScore());
            }
        }

        private IEnumerator SubscribeToEnemyScore()
        {
            // 상대 플레이어가 생성될 때까지 기다림
            yield return new WaitUntil(() =>
                NetworkManager.Singleton != null &&
                NetworkManager.Singleton.ConnectedClients.Count == 2);

            ulong myClientId = NetworkManager.Singleton.LocalClientId;

            foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
            {
                if (kvp.Key != myClientId)
                {
                    _enemyStatus = kvp.Value.PlayerObject.GetComponent<CharacterStatusNetwork>();
                    if (_enemyStatus != null)
                    {
                        SetEnemyScore(0, _enemyStatus.scoreBalls.Value);
                        _enemyStatus.scoreBalls.OnValueChanged -= SetEnemyScore;
                        _enemyStatus.scoreBalls.OnValueChanged += SetEnemyScore;
                    }
                    else
                    {
                        Debug.LogWarning("Enemy CharacterStatusNetwork not found.");
                    }
                    break;
                }
            }
        }

        private void OnDisable()
        {
            if (!_isOwner)
                return;

            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH, ActivateSpeedLine);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH_END, InActivateSpeedLine);
            _Owner.characterStatusNetwork.tagBalls.OnValueChanged   -= SetTagBallCount;
            _Owner.characterStatusNetwork.scoreBalls.OnValueChanged -= SetPlayerScore;
            _enemyStatus.scoreBalls.OnValueChanged                  -= SetEnemyScore;
        }

        private void ActivateSpeedLine()
        {
            if (_targetFeature != null)
                _targetFeature.SetActive(true);
        }

        private void InActivateSpeedLine()
        {
            if (_targetFeature != null)
                _targetFeature.SetActive(false);
        }
    }
}
