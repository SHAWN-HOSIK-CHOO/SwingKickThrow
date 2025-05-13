using Unity.Netcode;
using UnityEngine;

namespace GameFlow
{
   public class GameStartState : BaseGameState
   {
      public override void LateSetUp()
      {
         
      }

      public override void Handle()
      {
         if(NetworkManager.Singleton.IsHost)
         {
            foreach (var kvp in NetworkManager.Singleton.ConnectedClientsList)
            {
               ulong id = kvp.ClientId;
               GameSceneManager.Instance.SpawnMe(id);
            }
            
            Context.EnterState(GameStateType.GAME_BOX_GEN);
            Finish(GameStateType.GAME_START);
         }
      }

      public override void OnExit()
      {
      
      }
   }
}
