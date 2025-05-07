using Character;
using FSM;
using GameEvents;
using UnityEngine;

namespace CharacterAttributes
{
    public class HitState : BaseState
    {
        private CharacterController _characterController;
        private CharacterStatusNetwork _characterStatusNetwork;

        private float _pushedTime;
        private float _elapsedTime;
        private float _pushedSpeed;
        private Vector3 _moveDirection;
        IFSM<CharacterState, CharacterControllerNetwork> _characterFSM;
        public override void Setup(CharacterFSMContext context, CharacterControllerNetwork owner)
        {
            base.Setup(context, owner);
            _characterController = Owner.characterController;
            _characterStatusNetwork = Owner.characterStatusNetwork;
            _characterFSM = Owner.characterFSM;
        }

        public override void OnEnter()
        {
            _pushedTime = _characterStatusNetwork.pushedTime.Value;
            _elapsedTime = 0f;
            _moveDirection = -Owner.transform.forward;
            _pushedSpeed = _characterStatusNetwork.pushedSpeed.Value;

        }


        public void UpdateHit()
        {
            if (_elapsedTime <= _pushedTime)
            {
                // 충돌 검사
                float checkDistance = _pushedSpeed * Time.deltaTime;
                float checkRadius = 0.3f; // 캐릭터의 반지름 정도로 설정
                Ray ray = new Ray(Owner.transform.position + Vector3.up * 0.5f, _moveDirection);

                if (Physics.SphereCast(ray, checkRadius, out RaycastHit hit, checkDistance, ~0, QueryTriggerInteraction.Ignore))
                {
                    // 장애물에 부딪힘 → 즉시 멈춤
                    Finish(FSMType.HIT);
                    return;
                }

                // 이동
                Vector3 displacement = _moveDirection * (_pushedSpeed * Time.deltaTime);
                _characterController.Move(displacement);

                // 회전 (빙글빙글)
                float rotationSpeed = 720f; // 도/초
                Owner.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

                _elapsedTime += Time.deltaTime;
            }
            else
            {
                Finish(FSMType.HIT);
            }
        }
        public override void OnUpdate()
        {
            _characterFSM.TriggerTransition(CharacterState.Hit,true);
            _characterFSM.OnUpdate();
        }

        public override void OnExit()
        {
            GameEvents.GameEvents.Publish(GameEventType.PLAYER_CRASH_END);
        }
    }
}
