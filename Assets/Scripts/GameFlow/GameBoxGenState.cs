using System.Collections.Generic;
using GameProps;
using Unity.Netcode;
using UnityEngine;

namespace GameFlow
{
   public class GameBoxGenState : BaseGameState
   {
      private List<Transform>  _boxTransforms;
      //private List<GameObject> _boxGameObjects;
      public  GameObject pfBoxSpawner;
      private BoxSpawner _boxSpawner;

      private GameObject _pfBox;
      
      public override void SetUp(GameSceneFlowContext gameSceneFlowContext)
      {
         base.SetUp( gameSceneFlowContext);
         _boxTransforms = GameSceneManager.Instance.boxTransforms;
         _pfBox         = GameSceneManager.Instance.pfBoxObjects[0];
         pfBoxSpawner   = GameSceneManager.Instance.pfBoxSpawner;
      }

      public override void LateSetUp()
      {
         if (NetworkManager.Singleton == null)
         {
            Debug.Log("NetworkManager is null");
            return;
         }
         
         if (NetworkManager.Singleton.IsServer)
         {
            var go = GameObject.Instantiate(pfBoxSpawner);
            _boxSpawner = go.GetComponent<BoxSpawner>();
            _boxSpawner.Initialize(_boxTransforms, _pfBox);
            
            go.GetComponent<NetworkObject>().Spawn();
         }
      }

      public override void Handle()
      {
         if (NetworkManager.Singleton.IsServer)
         {
            if (_boxSpawner.IsSpawned) 
            {
               _boxSpawner.SpawnBox();
               Finish(GameStateType.GAME_BOX_GEN);
            }
            else
            {
               Debug.Log("BoxSpawner is not spawned yet");
            }
         }
      }

      public override void OnExit()
      {
      
      }
   }
}
