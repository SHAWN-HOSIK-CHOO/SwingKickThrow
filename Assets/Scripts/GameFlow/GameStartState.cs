using System.Threading;
using Unity.Netcode;
using UnityEngine;

namespace GameFlow
{
   public class GameStartState : BaseGameState
    {
      GameTimer timer;
      public override void LateSetUp()
      {
            timer = GameSceneManager.Instance.gameObject.GetComponent<GameTimer>();
            timer.OnElapsedTimeChanged -= GameSceneManager.Instance.UpdateTime;
            timer.OnElapsedTimeChanged += GameSceneManager.Instance.UpdateTime;
            GameSceneManager.Instance.winnerUI.SetActive(false);
      }

      public override void Handle()
      {
         if(NetworkManager.Singleton.IsHost)
         {
            foreach (var kvp in NetworkManager.Singleton.ConnectedClientsList)
            {
               ulong id = kvp.ClientId;
               GameSceneManager.Instance.SpawnMe(id);
            }
            Context.EnterState(GameStateType.GAME_BOX_GEN);
         }

         GameEvents.GameEvents.Subscribe(GameEvents.GameEventType.GameTimerEnd, GameTimerEnd);
         timer.StartCountDown();
         Finish(GameStateType.GAME_START);
      }

      public override void OnExit()
      {
         
      }

      private void GameTimerEnd()
      {
            GameEvents.GameEvents.Unsubscribe(GameEvents.GameEventType.GameTimerEnd, GameTimerEnd);
            Context.EnterState(GameStateType.GAME_END);
            Finish(GameStateType.GAME_START);
      }
    }
}
