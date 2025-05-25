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
using System.Runtime.InteropServices.WindowsRuntime;
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
                GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_CRASH,OnPlayerCrashShowDamage);

                tagBallCountText = GameSceneManager.Instance.tagBallCountText;
                playerScoreText  = GameSceneManager.Instance.playerScoreText;
                enemyScoreText   = GameSceneManager.Instance.enemyScoreText;
                damageText       = GameSceneManager.Instance.damageText;
                damageText.gameObject.SetActive(false);
                healthFill = GameSceneManager.Instance.healthFill;

                SubscribeHealth();
                healthFill.fillAmount = _owner.characterStatusNetwork.currentHealth.Value / _owner.characterStatusNetwork.maxHealth;
                
                if (enemyScoreText == null)
                    Debug.LogWarning("enemyScoreText is null! Check GameSceneManager setup.");

                _dashChargeTimer = _owner.dashTimer;

                _owner.characterStatusNetwork.tagBalls.OnValueChanged -= SetTagBallCount;
                _owner.characterStatusNetwork.tagBalls.OnValueChanged += SetTagBallCount;
                _owner.characterStatusNetwork.scoreBalls.OnValueChanged -= SetPlayerScore;
                _owner.characterStatusNetwork.scoreBalls.OnValueChanged += SetPlayerScore;

                SetPlayerScore(0, _owner.characterStatusNetwork.scoreBalls.Value);
                SetTagBallCount(0, _owner.characterStatusNetwork.tagBalls.Value);

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
            yield return new WaitUntil(() => GameSceneManager.Instance.playerReady.ReadyList.Count == GameSceneManager.Instance.NumPlayer);
            yield return new WaitUntil(() =>
            {
                foreach (var kvp in GameSceneManager.Instance.playerReady.ReadyList)
                {
                    if (!kvp.IsReady)
                    {
                        return false;
                    }
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
            if (chargeImage == null)
                return;
            chargeImage.fillAmount = _dashChargeTimer.GetChargedRatio();
        }

        private void OnEnable()
        {
            if (!_isOwner || !IsSpawned)
                return;
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH, ActivateSpeedLine);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH_END, InActivateSpeedLine);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_CRASH, OnPlayerCrashShowDamage);

            _owner.characterStatusNetwork.scoreBalls.OnValueChanged -= SetPlayerScore;
            _owner.characterStatusNetwork.scoreBalls.OnValueChanged += SetPlayerScore;
            _owner.characterStatusNetwork.tagBalls.OnValueChanged -= SetTagBallCount;
            _owner.characterStatusNetwork.tagBalls.OnValueChanged += SetTagBallCount;
            _enemyStatus.scoreBalls.OnValueChanged -= SetEnemyScore;
            _enemyStatus.scoreBalls.OnValueChanged += SetEnemyScore;
            SubscribeHealth();
        }


        private void OnDisable()
        {
            if (!_isOwner || !IsSpawned)
                return;
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH, ActivateSpeedLine);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH_END, InActivateSpeedLine);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_CRASH, OnPlayerCrashShowDamage);
            _owner.characterStatusNetwork.tagBalls.OnValueChanged   -= SetTagBallCount;
            _owner.characterStatusNetwork.scoreBalls.OnValueChanged -= SetPlayerScore;
            _enemyStatus.scoreBalls.OnValueChanged                  -= SetEnemyScore;
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

            // 예시 데미지 값 (원한다면 인자로 받아도 됨)
            float damage = _owner.myCurrentDamage;
            damageText.text = damage.ToString(CultureInfo.InvariantCulture);

            // RectTransform 가져오기
            RectTransform rectTransform = damageText.GetComponent<RectTransform>();

            // 위치와 투명도 초기화
            rectTransform.anchoredPosition = new Vector2(0, 100); // 예: 화면 중앙 위쪽
            damageText.color               = Color.blue;      // 빨간색, 완전 불투명

            // 텍스트를 보이게 함
            damageText.gameObject.SetActive(true);

            // DOTween 시퀀스
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPosY(150f, 1f).SetEase(Ease.OutCubic)); // 위로 50만큼 이동
            seq.Join(damageText.DOFade(0f, 1f));                                     // 동시에 페이드아웃

            seq.OnComplete(() =>
                           {
                               // 애니메이션 끝나면 텍스트 숨기기
                               damageText.gameObject.SetActive(false);

                               // 색상 초기화 (다음 재사용 대비)
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
