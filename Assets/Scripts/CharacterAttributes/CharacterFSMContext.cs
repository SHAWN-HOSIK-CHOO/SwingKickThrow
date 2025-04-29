using System.Collections.Generic;
using UnityEngine;

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
         //statePool[FSMType.HIT]      = new HitState();
         //statePool[FSMType.SKILL]    = new SkillState();
         //statePool[FSMType.DEATH]    = new DeathState();

         foreach (var state in statePool.Values)
            state.Setup(this, _owner);
      }

      public void SetState(FSMType type) 
      {
         if (currentStates.TryGetValue(type, out var current)) 
         {
            current.OnExit();
            Debug.Log($"FSMType : {type.ToString()} has exited");
         }

         Debug.Log($"{type.ToString()} is added to current states");
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
            Debug.Log($"{type.ToString()} is removed from current states : {!IsStateActive(type)}");
         }
      }

      public bool IsStateActive(FSMType type)
      {
         return currentStates.ContainsKey(type);
      }

      
      public void OnUpdate() 
      {
         // 실행 순서 명시
         //if (currentStates.ContainsKey(FSMType.Hit)) currentStates[FSMType.Hit].OnUpdate();
         //if (currentStates.ContainsKey(FSMType.Skill)) currentStates[FSMType.Skill].OnUpdate();
         if (currentStates.ContainsKey(FSMType.ATTACK)) currentStates[FSMType.ATTACK].OnUpdate();
         if (currentStates.ContainsKey(FSMType.MOVEMENT)) currentStates[FSMType.MOVEMENT].OnUpdate();
      }
      
   }
}
