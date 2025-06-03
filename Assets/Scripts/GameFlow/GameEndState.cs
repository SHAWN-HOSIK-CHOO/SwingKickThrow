using CharacterAttributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFlow
{
    public class GameEndState : BaseGameState
    {
        public override void SetUp(GameSceneFlowContext gameSceneFlowContext)
        {
            base.SetUp(gameSceneFlowContext);
        }
        public override void LateSetUp()
        {

        }
        void OnExitGame()
        {
            GameSceneManager.Instance.gameTimer = null;
            GameSceneManager.Instance._playerSelectionAndSpawnHandler = null;
            GameSceneManager.Instance.playerReady = null;
            GameSceneManager.Instance.UI.joinText.text = null;
            //index 99 to stop
            GameSceneManager.Instance.PlayAudio(99,99);
            Finish(GameStateType.GAME_END);
            if (NetworkManager.Singleton.IsHost)
            {
                var spawnedObjects = NetworkManager.Singleton.SpawnManager.SpawnedObjectsList.ToList();

                foreach (var obj in spawnedObjects)
                {
                    if (obj != null && obj.IsSpawned)
                        obj.Despawn(true);
                }
                NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            }
        }

        void UpdateResultUI()
        {
            var rankingList = new List<(ulong clientId, int score)>();

            if(NetworkManager.Singleton.ConnectedClients.Count != GameSceneManager.Instance.NumPlayer)
            {
                Debug.LogWarning("Player Exit Before Scoring");
                return;
            }

            foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
            {
                var playerObject = kvp.Value.PlayerObject;
                var status = playerObject?.GetComponent<CharacterStatusNetwork>();
                if (status != null)
                {
                    rankingList.Add((kvp.Key, status.scoreBalls.Value));
                }
                else
                {
                    Debug.LogWarning($"Client {kvp.Key} has no CharacterStatusNetwork.");
                }
            }
            rankingList.Sort((a, b) => b.score.CompareTo(a.score));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rankingList.Count; i++)
            {
                var entry = rankingList[i];
                sb.AppendLine($"{i + 1}: Player {entry.clientId} - {entry.score}");
            }
            GameSceneManager.Instance.UI.winnerUI.GetComponentInChildren<TMP_Text>().text = sb.ToString();
        }
        public override void Handle()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
            GameSceneManager.Instance.UI.winnerUI.gameObject.SetActive(true);
            GameSceneManager.Instance.UI.exit.onClick.AddListener(OnExitGame);
            UpdateResultUI();
        }

        public override void OnExit()
        {
            GameSceneManager.Instance.UI.winnerUI.gameObject.SetActive(false);
            GameSceneManager.Instance.UI.exit.onClick.RemoveListener(OnExitGame);
        }
    }
}
