using GameFlow;
using UnityEngine;

namespace CharacterAttributes
{
   public class DeathState : BaseState
   {
      private CharacterController _characterController;
      private float               _respawnTime;
      private float               _elapsedTime;
      private bool                _respawned;
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
         _respawned                   = false;
         Owner.serverCharacter.SetPlayerVisibleServerRPC(false);
         Owner.dashTimer.InitDashChargedFlag();
         //GameSceneManager.Instance.RespawnMe(Owner.OwnerClientId, Owner);
         //Owner.ResetHealth();
      }

      public override void OnUpdate()
      {
         _elapsedTime += Time.deltaTime;

         //Debug.Log($"elapsed time is : {_elapsedTime}");
         
         if ((_elapsedTime >= _respawnTime) && !_respawned)
         {
            _respawned = true;
            //Debug.Log($"After {_elapsedTime}");
            GameSceneManager.Instance.RespawnMe(Owner.OwnerClientId, Owner);
            Owner.ResetHealth();
         }

         if (_elapsedTime >= _respawnTime + 1f) 
         {
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
