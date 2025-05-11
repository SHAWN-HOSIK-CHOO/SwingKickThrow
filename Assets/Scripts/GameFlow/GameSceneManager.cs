using System;
using System.Collections.Generic;
using CharacterAttributes;
using ConnectionUtils;
using TMPro;
using UnityEngine;

namespace GameFlow
{
    [RequireComponent(typeof(GameTimer))]
    public class GameSceneManager : MonoBehaviour
    {
        private static GameSceneManager _instance = null;
        public static  GameSceneManager Instance => _instance == null ? null : _instance;
        
        private GameSceneFlowContext _gameSceneFlowContext;

        [Header("Box Transforms")] 
        public List<Transform> boxTransforms;
        
        [Header("Box Objects")] 
        public List<GameObject> pfBoxObjects;

        [Header("Player spawn Transforms")] 
        public Transform[] spawnTransforms;

        [Header("Player prefabs")] 
        public GameObject[] pfPlayers;

        //PlayerSelectionHandler 본인이 스스로 세팅
        private PlayerSelectionAndSpawnHandler _playerSelectionAndSpawnHandler;

        #region UI

        [Header("Tag ball")] 
        public TMP_Text tagBallCountText;

        [Header("Score")] 
        public TMP_Text playerScoreText;
        public TMP_Text enemyScoreText;
        #endregion
        
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

                if ((int)id >= spawnTransforms.Length)
                {
                    Debug.LogError("Overflow");
                }
                
                Transform spawnTransform = spawnTransforms[(int)id];
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
            
            _gameSceneFlowContext = new GameSceneFlowContext();
            _gameSceneFlowContext.Initialize(this);
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
    }
}
