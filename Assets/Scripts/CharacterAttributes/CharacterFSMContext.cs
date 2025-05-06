using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace CharacterAttributes
{
   public class CharacterFSMContext
   {
      public Dictionary<FSMType, BaseState> statePool     = new();
      public Dictionary<FSMType, BaseState> currentStates = new();

      private readonly CharacterControllerNetwork _owner;

      public CharacterFSMContext(CharacterControllerNetwork owner) {
         this._owner = owner;
      }

      public void Initialize() 
      {
         statePool[FSMType.MOVEMENT] = new MovementState();
         statePool[FSMType.ATTACK]   = new AttackState();
         statePool[FSMType.HIT]      = new HitState();
         statePool[FSMType.SKILL]    = new SkillState();
         //statePool[FSMType.DEATH]    = new DeathState();

         foreach (var state in statePool.Values)
            state.Setup(this, _owner);
      }

      public void SetState(FSMType type) 
      {
         if(!_owner.IsOwner)
            return;
         
         if (currentStates.TryGetValue(type, out var current)) 
         {
            current.OnExit();
            //Debug.Log($"FSMType : {type.ToString()} has exited");
         }

         //Debug.Log($"{type.ToString()} is added to current states");
         var next = statePool[type];
         currentStates[type] = next;
         next.OnEnter();
      }

      public void FinishState(FSMType type) 
      {
         if (currentStates.ContainsKey(type)) 
         {
            currentStates[type].OnExit();
            currentStates.Remove(type);
            //Debug.Log($"{type.ToString()} is removed from current states : {!IsStateActive(type)}");
         }

         if (type is FSMType.HIT or FSMType.SKILL)
         {
            SetState(FSMType.MOVEMENT);
            //Debug.Log("Movement state is activated after hit state exit");
         }
      }

      public void FinishAllStates()
      {
         foreach (var state in currentStates.Values)
         {
            state.OnExit();
         }

         currentStates.Clear();
         //Debug.Log("All states have been finished and cleared.");
      }
      
      public bool IsStateActive(FSMType type)
      {
         return currentStates.ContainsKey(type);
      }

      
      public void OnUpdate() 
      {
         // 실행 순서 명시
         if (currentStates.ContainsKey(FSMType.HIT)) currentStates[FSMType.HIT].OnUpdate();
         if (currentStates.ContainsKey(FSMType.SKILL)) currentStates[FSMType.SKILL].OnUpdate();
         if (currentStates.ContainsKey(FSMType.ATTACK)) currentStates[FSMType.ATTACK].OnUpdate();
         if (currentStates.ContainsKey(FSMType.MOVEMENT)) currentStates[FSMType.MOVEMENT].OnUpdate();
      }
      
   }
}
