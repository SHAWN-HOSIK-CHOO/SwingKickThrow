using GameProps;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace GameFlow
{
   public class GameStartState : BaseGameState
   {
        private BoxSpawner _boxSpawner;
        private GameUI _UI;
        private GameTimer _timer;
        private float boxGenElapsed;
        private float lastUpdateTime;
        public override void SetUp(GameSceneFlowContext gameSceneFlowContext)
        {
            base.SetUp(gameSceneFlowContext);

            _UI = GameSceneManager.Instance.UI;
            
        }
        public override void LateSetUp()
        {

        }

      public override void Handle()
      { 
          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible   = false;
          boxGenElapsed = GameSceneManager.Instance.BoxGenDelay;
         lastUpdateTime = 0;
         GameSceneManager.Instance.UI.winnerUI.SetActive(false);
         _timer = GameSceneManager.Instance.gameTimer;
         _boxSpawner = GameSceneManager.Instance._boxSpawner;
         GameEvents.GameEvents.Subscribe(GameEvents.GameEventType.GameEnd, GameTimerEnd);
         GameEvents.GameEvents.Subscribe(GameEvents.GameEventType.GameUpdate, GameTimerUpdate);
         GameSceneManager.Instance.gameTimer.StartCountDown();
         GameEvents.GameEvents.Publish(GameEvents.GameEventType.GameStart);
         GameSceneManager.Instance.PlayAudio(0,0);
         Finish(GameStateType.GAME_START);
      }

      public override void OnExit()
      {
         
      }
     private void GameTimerUpdate()
     {
            var deltaTime = _timer._elapsedTime.Value - lastUpdateTime;
            lastUpdateTime = _timer._elapsedTime.Value;
            //GenBox
            if (NetworkManager.Singleton.IsServer)
            {
                if (_boxSpawner.IsSpawned)
                {
                    boxGenElapsed += deltaTime;
                    if (boxGenElapsed >= GameSceneManager.Instance.BoxGenDelay)
                    {
                        boxGenElapsed = 0;
                        _boxSpawner.SpawnBox();
                        Debug.Log("Spawned Box" + lastUpdateTime);   
                    }
                }
            }

            // Update UI 
            float leftSeconds = Mathf.Clamp(_timer.requestedTime.Value - _timer._elapsedTime.Value, 0, _timer.requestedTime.Value);
            int minutes = (int)(leftSeconds / 60);
            int seconds = (int)(leftSeconds % 60);
            string timeFormatted = $"{minutes}:{seconds:D2}";
            _UI.timeText.text = timeFormatted;
        }
     
      private void GameTimerEnd()
        {
            GameEvents.GameEvents.Unsubscribe(GameEvents.GameEventType.GameUpdate, GameTimerUpdate);
            GameEvents.GameEvents.Unsubscribe(GameEvents.GameEventType.GameEnd, GameTimerEnd);
            Finish(GameStateType.GAME_START);
            Context.EnterState(GameStateType.GAME_END);
      }
    }
}
