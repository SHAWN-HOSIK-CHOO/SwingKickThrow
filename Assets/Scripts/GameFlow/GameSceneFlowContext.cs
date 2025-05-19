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
        NONE,
        GAME_START,
        GAME_IDLE,
        GAME_END,
        GAME_PAUSE,
        GAME_RESUME,
        GAME_BOX_GEN,
        COUNT
    }

    public class GameSceneFlowContext
    {
        public Dictionary<GameStateType, BaseGameState>   statePool    = new();
        public Dictionary<GameStateType, BaseGameState> currentState = new();
        public GameSceneManager                    Owner;

        public void Initialize()
        {
            //TODO: Initialize all states
            statePool[GameStateType.GAME_START]   = new GameStartState();
            statePool[GameStateType.GAME_IDLE]    = new GameIdleState();
            statePool[GameStateType.GAME_END]     = new GameEndState();
            statePool[GameStateType.GAME_BOX_GEN] = new GameBoxGenState();

            statePool[GameStateType.NONE] = new GameNoneState();
            
            foreach (var state in statePool)
            {
                state.Value.SetUp(this);
            }

            currentState[GameStateType.NONE] = statePool[GameStateType.NONE];
        }

        public void EnterState(GameStateType type)
        {
            if (currentState.TryGetValue(type, out var current))
            {
                current.OnExit();
            }

            var next = statePool[type];
            currentState[type] = next;
            next.Handle();
        }

        public void FinishState(GameStateType type)
        {
            if (currentState.ContainsKey(type))
            {
                currentState[type].OnExit();
                currentState.Remove(type);
                Debug.Log($"Removed {type.ToString()} state");
            }
            else
            {
                Debug.Log($"{type.ToString()} is not activated type! You cannot finish deactivated type");
            }
        }

        public void LateInitialize()
        {
            foreach (var state in statePool)
            {
                state.Value.LateSetUp();
            }
        }
    }
}