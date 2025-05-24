using UnityEngine;
using System.Collections.Generic;

namespace GameFlow
{
    /*
     * GAME_Wait - Spawn
     * GAME_START - Update Game
     * GAME_END - Show Result
     */
    public enum GameStateType
    {
        NONE,
        Game_WAIT,
        GAME_START,
        GAME_END,
        GAME_PAUSE,
        GAME_RESUME,
    }

    public class GameSceneFlowContext
    {
        public Dictionary<GameStateType, BaseGameState>   statePool    = new();
        public Dictionary<GameStateType, BaseGameState> currentState = new();
        public GameSceneManager                    Owner;

        public void Initialize()
        {
            //TODO: Initialize all states
            statePool[GameStateType.Game_WAIT]    = new GameWaitState();
            statePool[GameStateType.GAME_START]   = new GameStartState();
            statePool[GameStateType.GAME_END]     = new GameEndState();
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