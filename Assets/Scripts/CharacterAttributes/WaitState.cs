using UnityEngine;
using UnityEngine.InputSystem;

namespace CharacterAttributes
{
    public class WaitState : BaseState
    {
        // 아무것도 하지 않음
        public override void OnEnter()
        {
            Owner.playerInput.enabled = false;

            GameEvents.GameEvents.Subscribe(GameEvents.GameEventType.GameStart, OnGameStart);
        }

        public override void OnExit()
        {
            GameEvents.GameEvents.Unsubscribe(GameEvents.GameEventType.GameStart, OnGameStart);
        }

        void OnGameStart()
        {
            Context.SetState(FSMType.MOVEMENT);
            Finish(FSMType.WAIT);
        }

        public override void OnUpdate()
        {

        }

    }
}