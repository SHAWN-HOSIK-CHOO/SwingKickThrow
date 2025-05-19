using UnityEngine;

namespace CharacterAttributes
{
   public enum FSMType
   {
      WAIT,
      MOVEMENT,
      ATTACK, 
      HIT, 
      SKILL, 
      DEATH, 
      RESPAWN, 
      Count
   }

   public abstract class BaseState : IState
   {
      protected CharacterFSMContext        Context;
      protected CharacterControllerNetwork Owner;

      public virtual void Setup(CharacterFSMContext context, CharacterControllerNetwork owner)
      {
         this.Context = context;
         this.Owner   = owner;
      }
      
      public abstract void OnEnter();
      public abstract void OnUpdate();
      public abstract void OnExit();

      protected void Finish(FSMType type) 
      {
         Context.FinishState(type);
      }
   }
}