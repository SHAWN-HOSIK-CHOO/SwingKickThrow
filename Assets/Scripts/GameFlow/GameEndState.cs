using CharacterAttributes;
using ConnectionUtils;
using System.Collections.Generic;
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
        void onExit()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
            }
            Scene scene = SceneManager.GetSceneByName("Game_Main_Flow_Additive");
            if (scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
            Finish(GameStateType.GAME_END);
            Context.EnterState(GameStateType.NONE);
        }

        void UpdateResultUI()
        {
            var rankingList = new List<(ulong clientId, int score)>();
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
            // 점수 기준으로 내림차순 정렬
            rankingList.Sort((a, b) => b.score.CompareTo(a.score));
            // 텍스트 구성
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rankingList.Count; i++)
            {
                var entry = rankingList[i];
                sb.AppendLine($"{i + 1}: Player {entry.clientId} - {entry.score}");
            }
            // 출력
            GameSceneManager.Instance.UI.winnerUI.GetComponentInChildren<TMP_Text>().text = sb.ToString();
        }
        public override void Handle()
        {
            GameSceneManager.Instance.UI.winnerUI.gameObject.SetActive(true);
            GameSceneManager.Instance.UI.exit.onClick.AddListener(onExit);
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientsDisConnected;
            UpdateResultUI();
        }

        private void OnClientsDisConnected(ulong obj)
        {
            onExit();
        }

        public override void OnExit()
        {
            if(NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientsDisConnected;
            }
            GameSceneManager.Instance.UI.winnerUI.gameObject.SetActive(false);
            GameSceneManager.Instance.UI.exit.onClick.RemoveListener(onExit);
        }
    }
}
