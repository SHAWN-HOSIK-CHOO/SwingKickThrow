using System.Threading;
using Unity.Netcode;
using UnityEngine;

namespace GameFlow
{
   public class GameStartState : BaseGameState
   {
      public override void LateSetUp()
      {

      }

      public override void Handle()
      {
         GameSceneManager.Instance.UI.winnerUI.SetActive(false);
         if (NetworkManager.Singleton.IsHost)
         {
            foreach (var kvp in NetworkManager.Singleton.ConnectedClientsList)
            {
               ulong id = kvp.ClientId;
               GameSceneManager.Instance.SpawnMe(id);
            }
            Context.EnterState(GameStateType.GAME_BOX_GEN);
         }

         GameEvents.GameEvents.Subscribe(GameEvents.GameEventType.GameEnd, GameTimerEnd);
         GameSceneManager.Instance.gameTimer.StartCountDown();
         GameEvents.GameEvents.Publish(GameEvents.GameEventType.GameStart);
         Finish(GameStateType.GAME_START);
      }

      public override void OnExit()
      {
         
      }

      private void GameTimerEnd()
      {
            GameEvents.GameEvents.Unsubscribe(GameEvents.GameEventType.GameEnd, GameTimerEnd);
            Context.EnterState(GameStateType.GAME_END);
            Finish(GameStateType.GAME_START);
      }
    }
}
