using GameFlow;
using UnityEngine;

namespace CharacterAttributes
{
   public class DeathState : BaseState
   {
      private CharacterController _characterController;
      private float               _respawnTime;
      private float               _elapsedTime;
      public override void Setup(CharacterFSMContext context, CharacterControllerNetwork owner)
      {
         base.Setup(context, owner);
         _characterController = Owner.characterController;
         _respawnTime         = Owner.timeUntilRespawn;
      }

      public override void OnEnter()
      {
         Debug.Log("Entered Death state");
         _characterController.enabled = false;
         _elapsedTime                 = 0f;
         Owner.serverCharacter.SetPlayerVisibleServerRPC(false);
         //GameSceneManager.Instance.RespawnMe(Owner.OwnerClientId, Owner);
         //Owner.ResetHealth();
      }

      public override void OnUpdate()
      {
         _elapsedTime += Time.deltaTime;

         //Debug.Log($"elapsed time is : {_elapsedTime}");
         
         if (_elapsedTime >= _respawnTime)
         {
            //Debug.Log($"After {_elapsedTime}");
            GameSceneManager.Instance.RespawnMe(Owner.OwnerClientId, Owner);
            Owner.ResetHealth();
            Finish(FSMType.DEATH);
         }
      }

      public override void OnExit()
      {
         // GameSceneManager.Instance.RespawnMe(Owner.OwnerClientId, Owner);
         // Owner.ResetHealth();
         Debug.Log("Finished Death state");
         _characterController.enabled = true;
         Owner.serverCharacter.SetPlayerVisibleServerRPC(true);
      }
   }
}
