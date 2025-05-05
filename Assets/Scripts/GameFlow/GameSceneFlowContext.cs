using UnityEngine;
using System.Collections.Generic;

namespace GameFlow
{
    /*
     * GAME_START에서는 Player Spawn
     * GAME_IDLE에서는 박스 관리
     * GAME_END에서는 종료 및 점수 결산
     */
    public enum GameStateType
    {
        GAME_START,
        GAME_IDLE,
        GAME_END,
        GAME_PAUSE,
        GAME_RESUME,
        GAME_BOX_GEN,
        Count
    }

    public class GameSceneFlowContext
    {
        public Dictionary<GameStateType, BaseGameState>   statePool    = new();
        public KeyValuePair<GameStateType, BaseGameState> currentState = new();
        public GameSceneFlowController                    Owner;

        public void Initialize(GameSceneFlowController gameSceneFlowController)
        {
            //TODO: Initialize all states
            statePool[GameStateType.GAME_START]   = new GameStartState();
            statePool[GameStateType.GAME_IDLE]    = new GameIdleState();
            statePool[GameStateType.GAME_END]     = new GameEndState();
            statePool[GameStateType.GAME_BOX_GEN] = new GameBoxGenState();
            
            Owner = gameSceneFlowController;

            foreach (var state in statePool)
            {
                state.Value.SetUp(Owner,this);
            }
        }

        public void EnterState(GameStateType type)
        {
            if (currentState.Key == type)
            {
                currentState.Value.OnExit();
            }

            var next = statePool[type];
            currentState = new KeyValuePair<GameStateType, BaseGameState>(type, next);
            currentState.Value.Handle();
        }

        public void FinishState(GameStateType type)
        {
            if (currentState.Key == type)
            {
                currentState.Value.OnExit();
            }
            else
            {
                Debug.Log($"{type.ToString()} is not activated type! You cannot finish deactivated type");
            }
        }
        
    }
}