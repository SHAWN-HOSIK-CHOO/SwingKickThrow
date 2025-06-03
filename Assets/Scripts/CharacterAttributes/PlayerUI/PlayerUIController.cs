using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using GameEvents;
using GameFlow;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Unity.Netcode;
using UnityEngine.UI;
using DG.Tweening;

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
            if (enemyScoreText != null)
                enemyScoreText.text = newVal.ToString();
            else
                Debug.LogWarning("enemyScoreText is null! Check GameSceneManager setup.");
        }

        #endregion

        #region ChargeStatus

        [FormerlySerializedAs("_chargeImage")]
        public GameObject pfChargeUI;
        public Image chargeImage;
        private DashChargeTimer _dashChargeTimer;
        private GameObject _chargeUI;

        #endregion

        #region DamageStatus

        public TMP_Text damageText;

        #endregion

        #region Health

        public Image healthFill;

        #endregion
        

        public void Initialize(CharacterControllerNetwork characterControllerNetwork)
        {

            _isOwner = characterControllerNetwork.IsOwner;
            if (_isOwner)
            {
                _owner = characterControllerNetwork;
                GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_DASH, ActivateSpeedLine);
                GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_DASH_END, InActivateSpeedLine);
                GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_CRASH, OnPlayerCrashShowDamage);

                tagBallCountText = GameSceneManager.Instance.tagBallCountText;
                playerScoreText = GameSceneManager.Instance.playerScoreText;
                enemyScoreText = GameSceneManager.Instance.enemyScoreText;
                damageText = GameSceneManager.Instance.damageText;
                damageText.gameObject.SetActive(false);
                healthFill = GameSceneManager.Instance.healthFill;

                SubscribeHealth();
                healthFill.fillAmount = _owner.characterStatusNetwork.currentHealth.Value / _owner.characterStatusNetwork.maxHealth;

                _dashChargeTimer = _owner.dashTimer;

                _owner.characterStatusNetwork.tagBalls.OnValueChanged -= SetTagBallCount;
                _owner.characterStatusNetwork.tagBalls.OnValueChanged += SetTagBallCount;
                _owner.characterStatusNetwork.scoreBalls.OnValueChanged -= SetPlayerScore;
                _owner.characterStatusNetwork.scoreBalls.OnValueChanged += SetPlayerScore;

                SetPlayerScore(0, _owner.characterStatusNetwork.scoreBalls.Value);
                SetTagBallCount(0, _owner.characterStatusNetwork.tagBalls.Value);

                _chargeUI = Instantiate(pfChargeUI, GameSceneManager.Instance.UI.DashCharge.transform);
                chargeImage = _chargeUI.transform.Find("DashChargeFill").GetComponent<Image>();

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
            yield return new WaitUntil(() => GameSceneManager.Instance.playerReady.ReadyList.Count == GameSceneManager.Instance.NumPlayer);
            yield return new WaitUntil(() =>
            {
                foreach (var kvp in GameSceneManager.Instance.playerReady.ReadyList)
                {
                    if (!kvp.IsReady) return false;
                }
                return true;
            });

            ulong myClientId = NetworkManager.Singleton.LocalClientId;
            foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
            {
                if (kvp.Key != myClientId)
                {
                    _enemyStatus = kvp.Value.PlayerObject.GetComponent<CharacterStatusNetwork>();
                    if (_enemyStatus != null)
                    {
                        _enemyStatus.scoreBalls.OnValueChanged -= SetEnemyScore;
                        _enemyStatus.scoreBalls.OnValueChanged += SetEnemyScore;
                        SetEnemyScore(0, _enemyStatus.scoreBalls.Value);
                    }
                }
            }
        }

        private void Update()
        {
            if (!IsSpawned) return;
            if (chargeImage == null) return;

            chargeImage.fillAmount = _dashChargeTimer.GetChargedRatio();
        }

        private void OnEnable()
        {
            if (!_isOwner || !IsSpawned) return;

            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH, ActivateSpeedLine);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH_END, InActivateSpeedLine);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_CRASH, OnPlayerCrashShowDamage);

            _owner.characterStatusNetwork.scoreBalls.OnValueChanged -= SetPlayerScore;
            _owner.characterStatusNetwork.scoreBalls.OnValueChanged += SetPlayerScore;
            _owner.characterStatusNetwork.tagBalls.OnValueChanged -= SetTagBallCount;
            _owner.characterStatusNetwork.tagBalls.OnValueChanged += SetTagBallCount;

            if (_enemyStatus != null)
            {
                _enemyStatus.scoreBalls.OnValueChanged -= SetEnemyScore;
                _enemyStatus.scoreBalls.OnValueChanged += SetEnemyScore;
            }

            SubscribeHealth();
        }

        private void OnDisable()
        {
            if (!_isOwner || !IsSpawned) return;

            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH, ActivateSpeedLine);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH_END, InActivateSpeedLine);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_CRASH, OnPlayerCrashShowDamage);

            _owner.characterStatusNetwork.tagBalls.OnValueChanged -= SetTagBallCount;
            _owner.characterStatusNetwork.scoreBalls.OnValueChanged -= SetPlayerScore;

            if (_enemyStatus != null)
            {
                _enemyStatus.scoreBalls.OnValueChanged -= SetEnemyScore;
            }
            
            UnsubscribeHealth();
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

        private void OnPlayerCrashShowDamage()
        {
            if (damageText == null) return;

            float damage = _owner.myCurrentDamage;
            damageText.text = damage.ToString(CultureInfo.InvariantCulture);

            RectTransform rectTransform = damageText.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, 100);
            damageText.color = Color.blue;
            damageText.gameObject.SetActive(true);

            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPosY(150f, 1f).SetEase(Ease.OutCubic));
            seq.Join(damageText.DOFade(0f, 1f));

            seq.OnComplete(() =>
            {
                damageText.gameObject.SetActive(false);
                damageText.color = Color.blue;
            });
        }

        private void SubscribeHealth()
        {
            var status = _owner.characterStatusNetwork;
            status.currentHealth.OnValueChanged -= OnHealthChanged;
            status.currentHealth.OnValueChanged += OnHealthChanged;
        }

        private void UnsubscribeHealth()
        {
            if (_owner != null && _owner.characterStatusNetwork != null)
            {
                _owner.characterStatusNetwork.currentHealth.OnValueChanged -= OnHealthChanged;
            }
        }

        private void OnHealthChanged(float prev, float current)
        {
            float max = _owner.characterStatusNetwork.maxHealth;
            healthFill.fillAmount = current / max;
        }
    }
}
