using CharacterAttributes;
using FSM;
using System.Globalization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class MoveController : IOwner<MoveController>
    {
        public CharacterController _characterController;
        private InputActionMap inputActions;

        public Vector2 MoveInput { get; set; }
        public Vector2 LookInput { get; set; }
        public bool JumpInput { get; set; }
        public bool AttackInput { get; set; }
        public bool SkillInput { get; set; }

        IFSM<MoveState,MoveController> _fsm;
        public MoveController Get => this;
        void Initialize(CharacterController characterConotroller)
        {
            _fsm = new MoveFSM<MoveController>(this);
            _characterController = characterConotroller;
        }

        public void BindInput()
        {
            inputActions["Move"].performed += OnMove;
            inputActions["Look"].performed += OnLook;
            inputActions["Attack"].performed += OnAttack;
            inputActions["Skill"].performed += OnSkill;
            inputActions["Jump"].performed += OnJump;
            inputActions.Enable();
        }
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            AttackInput = context.ReadValue<float>() == 1;
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            JumpInput = context.ReadValue<float>() == 1;
        }
        public void OnSkill(InputAction.CallbackContext context)
        {
            SkillInput = context.ReadValue<float>() == 1;
        }
    }
}