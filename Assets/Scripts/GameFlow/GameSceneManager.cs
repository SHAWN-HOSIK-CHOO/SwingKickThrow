using System;
using System.Collections;
using System.Collections.Generic;
using CharacterAttributes;
using ConnectionUtils;
using GameEvents;
using GameProps;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GameFlow
{
    public class GameSceneManager : MonoBehaviour
    {
        private static GameSceneManager _instance = null;
        public static  GameSceneManager Instance => _instance == null ? null : _instance;
        
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

        [Header("Full screen UI ex). Crashed UI")]
        public GameObject splashedScreenUI;
        public  GameObject playerCrashedScreenUI;
        public  GameObject enemyCrashedScreenUI;
        
        [HideInInspector]
        public GameObject currentPlayerCrashedUI;
        #endregion

        public GameObject pfBoxSpawner;
        
        public void SetPlayerSelectionHandler(PlayerSelectionAndSpawnHandler andSpawnHandler)
        {
            _playerSelectionAndSpawnHandler = andSpawnHandler;
        }

        public void SpawnMe(ulong id)
        {
            //Debug.Log($"Spawn Request from client {id}");
            _playerSelectionAndSpawnHandler.SpawnMeWrapper(id);
        }

        public void RespawnMe(ulong id, CharacterControllerNetwork player)
        {
            Debug.Log($"Respawn Request from client {id}");
            //_playerSelectionAndSpawnHandler.RespawnMeWrapper(id);
            if (player.IsOwner)
            {
                //Debug.Log("IsOwner passed from respawn method");

                // if ((int)id >= spawnTransforms.Length)
                // {
                //     Debug.LogError("Overflow");
                // }

                int spawnID = id == 0 ? 0 : 1;
                
                Transform spawnTransform = spawnTransforms[spawnID];
                player.transform.position =
                    new Vector3(spawnTransform.position.x, spawnTransform.position.y, spawnTransform.position.z);
                player.transform.rotation = spawnTransform.rotation;
            }
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
            currentPlayerCrashedUI = playerCrashedScreenUI;
            currentPlayerCrashedUI.gameObject.SetActive(false);
            splashedScreenUI.gameObject.SetActive(false);
            enemyCrashedScreenUI.gameObject.SetActive(false);
            playerCrashedScreenUI.gameObject.SetActive(false);
        }

        //클라이언트가 연결되고 수행되어야 할 설정들
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
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DEATH,DisplayPlayerCrashedScreenUI);
        }

        private Coroutine _displayCoroutine;
        
        private void DisplayPlayerCrashedScreenUI()
        {
            if (AnyScreenIsActive())
                return;
            
            if(_displayCoroutine != null)
                StopCoroutine(_displayCoroutine);

            _displayCoroutine = StartCoroutine(CoDisplayScreenUI(1));
        }

        private void DisplayEnemyCrashedScreenUI()
        {
            if (AnyScreenIsActive())
                return;
            
            if(_displayCoroutine != null)
                StopCoroutine(_displayCoroutine);

            _displayCoroutine = StartCoroutine(CoDisplayScreenUI(0));
        }

        private IEnumerator CoDisplayScreenUI(int num)
        {
            if (AnyScreenIsActive())
            {
                yield break; // 코루틴 종료
            }
            
            if (num == 0)
            {
                enemyCrashedScreenUI.gameObject.SetActive(true);
            }
            else if (num == 1)
            {
                currentPlayerCrashedUI.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(2.3f);
            
            playerCrashedScreenUI.gameObject.SetActive(false);
            splashedScreenUI.gameObject.SetActive(false);
            enemyCrashedScreenUI.gameObject.SetActive(false);
            currentPlayerCrashedUI.gameObject.SetActive(false);
        }

        private bool AnyScreenIsActive()
        {
            return playerCrashedScreenUI.gameObject.activeInHierarchy ||
                   splashedScreenUI.gameObject.activeInHierarchy      ||
                   currentPlayerCrashedUI.gameObject.activeInHierarchy;
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
            // Check
           if(leftSeconds == 0)
           {
                GameEvents.GameEvents.Publish(GameEvents.GameEventType.GameEnd);
           }
        }
        //디버그 용 update, 릴리즈에서는 지울 예정
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                try
                {
                    _boxSpawner.SpawnBox();
                }
                catch
                {

                }
            }
        }
    }
}
