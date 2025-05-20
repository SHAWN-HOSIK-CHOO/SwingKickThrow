using System;
using System.Collections;
using UnityEngine;
using GameEvents;
using GameFlow;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Unity.Netcode;
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;

namespace CharacterAttributes.PlayerUI
{
    public class PlayerUIController : NetworkBehaviour
    {
        public UniversalRendererData rendererData;
        private ScriptableRendererFeature _targetFeature;

        private CharacterControllerNetwork _owner;
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

        #region ChargeStatus

        [FormerlySerializedAs("_chargeImage")]
        public GameObject pfChargeUI;
        public  Image           chargeImage;
        private DashChargeTimer _dashChargeTimer;
        private GameObject      _chargeUI;

        #endregion

        public override void OnNetworkSpawn()
        {




        }

        public void Initialize(CharacterControllerNetwork characterControllerNetwork)
        {
            _isOwner = characterControllerNetwork.IsOwner;
            if (_isOwner)
            {
                _owner = characterControllerNetwork;
                GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_DASH, ActivateSpeedLine);
                GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_DASH_END, InActivateSpeedLine);

                tagBallCountText = GameSceneManager.Instance.tagBallCountText;
                playerScoreText = GameSceneManager.Instance.playerScoreText;
                enemyScoreText = GameSceneManager.Instance.enemyScoreText;

                if (enemyScoreText == null)
                    Debug.LogWarning("enemyScoreText is null! Check GameSceneManager setup.");

                _dashChargeTimer = _owner.dashTimer;

                SetTagBallCount(0, _owner.characterStatusNetwork.tagBalls.Value);
                _owner.characterStatusNetwork.tagBalls.OnValueChanged -= SetTagBallCount;
                _owner.characterStatusNetwork.scoreBalls.OnValueChanged -= SetPlayerScore;
                _owner.characterStatusNetwork.tagBalls.OnValueChanged += SetTagBallCount;
                _owner.characterStatusNetwork.scoreBalls.OnValueChanged += SetPlayerScore;

                _chargeUI = Instantiate(pfChargeUI, GameSceneManager.Instance.UI.DashCharge.transform);
                chargeImage = _chargeUI.transform.Find("DashChargeFill").GetComponent<Image>();
                // 적 점수 업데이트용 구독 시작
                StartCoroutine(SubscribeToEnemyScore());
            }

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
        }
        private IEnumerator SubscribeToEnemyScore()
        {

            yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClientsList.Count == GameSceneManager.Instance.NumPlayer);
            yield return new WaitUntil(() =>
            {
                ulong myClientId = NetworkManager.Singleton.LocalClientId;
                foreach (var kvp in GameSceneManager.Instance.playerReady.ReadyList)
                {
                    if (kvp.ClientId != myClientId)
                    {
                        return kvp.IsReady;
                    }
                }
                return false;
            });
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
                        break;
                    }
                    yield return null;
                }
            }
        }

        private void Update()
        {
            if (!IsSpawned) return;
            if (chargeImage == null)
                return;
            chargeImage.fillAmount = _dashChargeTimer.GetChargedRatio();
        }

        private void OnDisable()
        {
            if (!_isOwner || !IsSpawned)
                return;

            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH, ActivateSpeedLine);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH_END, InActivateSpeedLine);
            _owner.characterStatusNetwork.tagBalls.OnValueChanged   -= SetTagBallCount;
            _owner.characterStatusNetwork.scoreBalls.OnValueChanged -= SetPlayerScore;
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
