using CharacterAttributes;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace GameFlow
{
   public class GameEndState : BaseGameState
   {
      public override void LateSetUp()
      {
         
      }

      public override void Handle()
      {
            GameSceneManager.Instance.winnerUI.gameObject.SetActive(true);
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
            GameSceneManager.Instance.winnerUI.GetComponentInChildren<TMP_Text>().text = sb.ToString();
        }

      public override void OnExit()
      {
      
      }
   }
}
