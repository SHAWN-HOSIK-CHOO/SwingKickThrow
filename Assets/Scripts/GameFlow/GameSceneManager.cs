using System.Collections;
using System.Collections.Generic;
using CharacterAttributes;
using ConnectionUtils;
using GameEvents;
using GameProps;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Audio;

namespace GameFlow
{
    public class GameSceneManager : MonoBehaviour
    {
        private static GameSceneManager _instance = null;
        public static GameSceneManager Instance => _instance == null ? null : _instance;

        public GameSceneFlowContext gameSceneFlowContext;

        [Header("Box Transforms")] 
        public List<Transform> boxTransforms;

        [Header("Box Objects")] 
        public List<GameObject> pfBoxObjects;
        [HideInInspector]
        public BoxSpawner _boxSpawner;

        public float BoxGenDelay = 60f;

        [Header("Player spawn Transforms")] 
        public Transform[] spawnTransforms;

        [Header("Player prefabs")] 
        public GameObject[] pfPlayers;

        [Header("Player Count")]
        public int NumPlayer = 2;

        [Header("Play time")]
        public float gameTime;
        public PlayerReady playerReadyPf;
        public PlayerReady playerReady;
        public GameTimer gameTimerPf;
        [HideInInspector]
        public GameTimer gameTimer;
        public PlayerSelectionAndSpawnHandler playerSelectPf;
        [HideInInspector]
        public PlayerSelectionAndSpawnHandler _playerSelectionAndSpawnHandler;

        #region UI

        [Header("Tag ball")] 
        public TMP_Text tagBallCountText;

        [Header("Score")] 
        public TMP_Text playerScoreText;
        public TMP_Text enemyScoreText;

        [Header("Damage")]
        public TMP_Text damageText;

        [Header("Health fill")]
        public Image healthFill;

        [Header("UI")]
        public GameUI UI;

        [Header("ESC Panel")]
        public GameObject escPanel;
        public Slider volumeSlider;
        public Slider bgmVolumeSlider;
        public Slider sfxVolumeSlider;

        [Header("Full screen UI ex). Crashed UI")]
        public GameObject splashedScreenUI;
        public GameObject playerCrashedScreenUI;
        public GameObject enemyCrashedScreenUI;

        public GameObject currentPlayerCrashedUI;

        private RectTransform _splashRect;
        private RectTransform _playerCrashRect;
        private RectTransform _enemyCrashRect;

        private Queue<IEnumerator> _uiQueue = new Queue<IEnumerator>();
        private bool _isPlayingUI = false;

        #endregion

        #region Audio

        public  AudioClip[] gameBgMsAudioClips;
        private AudioSource _audioSource;

        public void SetVolume(float value)
        {
            Debug.Log("[Audio Slider] SetVolume called with value = " + value);
            AudioListener.volume = value;
        }
        
        public AudioMixer mainMixer;

        public void SetBGMVolume(float volume) // 0.0 ~ 1.0
        {
            float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
            mainMixer.SetFloat("BgMVolume", dB);
        }

        public void SetSfxVolume(float volume)
        {
            float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
            mainMixer.SetFloat("SFXVolume", dB);
        }
        
        #endregion
        
        public GameObject pfBoxSpawner;

        public void SetPlayerSelectionHandler(PlayerSelectionAndSpawnHandler andSpawnHandler)
        {
            _playerSelectionAndSpawnHandler = andSpawnHandler;
        }

        public void SpawnMe(ulong id)
        {
            _playerSelectionAndSpawnHandler.SpawnMeWrapper(id);
        }

        public void RespawnMe(ulong id, CharacterControllerNetwork player)
        {
            int spawnID = id == 0 ? 0 : 1;
            Transform spawnTransform = spawnTransforms[spawnID];
            player.transform.position = spawnTransform.position;
            player.transform.rotation = spawnTransform.rotation;
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }

            UI = GetComponent<GameUI>();
            gameSceneFlowContext = new GameSceneFlowContext();
            gameSceneFlowContext.Initialize();
        }

        private void Start()
        {
            _audioSource           = this.GetComponent<AudioSource>();
            currentPlayerCrashedUI = playerCrashedScreenUI;
            playerCrashedScreenUI.SetActive(false);
            splashedScreenUI.SetActive(false);
            enemyCrashedScreenUI.SetActive(false);

            _splashRect = splashedScreenUI.GetComponent<RectTransform>();
            _playerCrashRect = playerCrashedScreenUI.GetComponent<RectTransform>();
            _enemyCrashRect = enemyCrashedScreenUI.GetComponent<RectTransform>();
            
            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.RemoveListener(SetVolume);
                volumeSlider.onValueChanged.AddListener(SetVolume);
                volumeSlider.value = AudioListener.volume;
                Debug.Log("[UI Init] Volume slider listener added in Start()");
            }
            else
            {
                Debug.LogWarning("[UI Init] VolumeSlider is NULL in Start()");
            }
            
            if (bgmVolumeSlider != null)
            {
                bgmVolumeSlider.onValueChanged.RemoveListener(SetBGMVolume);
                bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
                bgmVolumeSlider.value = 0.5f; 
                SetBGMVolume(bgmVolumeSlider.value);
                Debug.Log("[UI Init] BGM volume slider listener added in Start()");
            }
            else
            {
                Debug.LogWarning("[UI Init] BGM volumeSlider is NULL in Start()");
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.onValueChanged.RemoveListener(SetSfxVolume);
                sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
                sfxVolumeSlider.value = 1f;
                SetSfxVolume(sfxVolumeSlider.value);
                Debug.Log("[UI Init] SFX volume slider listener added in Start()");
            }
            else
            {
                Debug.LogWarning("[UI Init] SFX volumeSlider is NULL in Start()");
            }
            
            escPanel.gameObject.SetActive(false);
        }

        public void PlayAudio(int index)
        {
            if (index == 99)
            {
                _audioSource.Stop();
                return;
            }
            
            if(_audioSource.isPlaying)
                _audioSource.Stop();
            
            _audioSource.clip = gameBgMsAudioClips[index];
            _audioSource.Play();
        }
        
        public void LateInitialize()
        {
            gameSceneFlowContext.LateInitialize();
        }

        private void OnEnable()
        {
            GameEvents.GameEvents.Subscribe(GameEventType.ENEMY_DEATH, DisplayEnemyCrashedScreenUI);
            GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_DEATH, DisplayPlayerCrashedScreenUI);
        }

        private void OnDisable()
        {
            GameEvents.GameEvents.Unsubscribe(GameEventType.ENEMY_DEATH, DisplayEnemyCrashedScreenUI);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DEATH, DisplayPlayerCrashedScreenUI);
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
        }

        private void DisplayPlayerCrashedScreenUI()
        {
            EnqueueUI(CoDisplayPlayerDeath());
        }

        private void DisplayEnemyCrashedScreenUI()
        {
            EnqueueUI(CoDisplayEnemyDeath());
        }

        private void EnqueueUI(IEnumerator routine)
        {
            _uiQueue.Enqueue(routine);
            if (!_isPlayingUI)
            {
                StartCoroutine(ProcessUIQueue());
            }
        }

        private IEnumerator ProcessUIQueue()
        {
            _isPlayingUI = true;

            while (_uiQueue.Count > 0)
            {
                var routine = _uiQueue.Dequeue();
                yield return StartCoroutine(routine);
            }

            _isPlayingUI = false;
        }

        private IEnumerator CoDisplayPlayerDeath()
        {
            currentPlayerCrashedUI.SetActive(true);
            yield return ShowBouncyUI(currentPlayerCrashedUI.GetComponent<RectTransform>());
            yield return new WaitForSeconds(2.3f);
            currentPlayerCrashedUI = playerCrashedScreenUI;
            currentPlayerCrashedUI.SetActive(false);
            splashedScreenUI.SetActive(false);
            playerCrashedScreenUI.SetActive(false);
        }

        private IEnumerator CoDisplayEnemyDeath()
        {
            enemyCrashedScreenUI.SetActive(true);
            yield return ShowBouncyUI(_enemyCrashRect);
            yield return new WaitForSeconds(2.3f);
            enemyCrashedScreenUI.SetActive(false);
        }

        private IEnumerator ShowBouncyUI(RectTransform rect)
        {
            rect.anchoredPosition = new Vector2(0, Screen.height);
            yield return rect.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBounce).WaitForCompletion();
        }

        public void SpawnPlayerSelect()
        {
            var playerSelect = Instantiate(playerSelectPf);
            playerSelect.GetComponent<NetworkObject>().Spawn();
        }

        public void SpawnBoxSpawner()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var go = GameObject.Instantiate(pfBoxSpawner);
                _boxSpawner = go.GetComponent<BoxSpawner>();
                _boxSpawner.Initialize(boxTransforms, pfBoxObjects[0]);
                go.GetComponent<NetworkObject>().Spawn();
            }
        }

        public void SpawnTimer()
        {
            gameTimer = Instantiate(gameTimerPf);
            gameTimer.GetComponent<NetworkObject>().Spawn();
            gameTimer.requestedTime.Value = gameTime;
        }

        public void SpawnPlayerReady()
        {
            var playerReady = Instantiate(playerReadyPf);
            playerReady.GetComponent<NetworkObject>().Spawn();
        }

        public void UpdateTime(float old, float elapsed)
        {
            float leftSeconds = Mathf.Clamp(gameTimer.requestedTime.Value - gameTimer._elapsedTime.Value, 0, gameTimer.requestedTime.Value);
            GameEvents.GameEvents.Publish(GameEvents.GameEventType.GameUpdate);
            if (leftSeconds == 0)
            {
                GameEvents.GameEvents.Publish(GameEvents.GameEventType.GameEnd);
            }
        }

        private bool _isPaused = false;

        private void TogglePauseMenu()
        {
            _isPaused = !_isPaused;
            escPanel.SetActive(_isPaused);
            Cursor.visible   = _isPaused;
            Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                try
                {
                    _boxSpawner.SpawnBox();
                }
                catch { }
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseMenu();
            }
        }
        
    }
}


