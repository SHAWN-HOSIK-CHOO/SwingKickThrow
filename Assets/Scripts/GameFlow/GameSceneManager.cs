using System;
using System.Collections.Generic;
using CharacterAttributes;
using ConnectionUtils;
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
        public GameTimer gameTimer;
        public PlayerSelectionAndSpawnHandler playerSelectPf;
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

        //클라이언트가 연결되고 수행되어야 할 설정들
        public void LateInitialize()
        {
            gameSceneFlowContext.LateInitialize();
        }
        
        private void OnEnable()
        {
            
        }
        
        private void OnDisable()
        {
            
        }

        private void OnBoxGenCalled()
        {
            
        }

        public void SpawnPlayerSelect()
        {
            var playerSelect = Instantiate(playerSelectPf);
            playerSelect.GetComponent<NetworkObject>().Spawn();
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
           // Update UI 
           int minutes = (int)(leftSeconds / 60);
           int seconds = (int)(leftSeconds % 60);
           string timeFormatted = $"{minutes}:{seconds:D2}";
           UI.timeText.text = timeFormatted;
           
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
                gameSceneFlowContext.EnterState(GameStateType.GAME_BOX_GEN);
            }
        }
    }
}
