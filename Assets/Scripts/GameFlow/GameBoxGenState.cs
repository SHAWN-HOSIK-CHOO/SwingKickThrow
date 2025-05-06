using System.Collections.Generic;
using UnityEngine;

namespace GameFlow
{
   public class GameBoxGenState : BaseGameState
   {
      private List<Transform>  _boxTransforms;
      private List<GameObject> _boxGameObjects;
      
      public override void SetUp(GameSceneManager gameSceneManager, GameSceneFlowContext gameSceneFlowContext)
      {
         base.SetUp(gameSceneManager, gameSceneFlowContext);
         _boxTransforms = Owner.boxTransforms;
      }

      public override void Handle()
      {
         
      }

      public override void OnExit()
      {
      
      }
   }
}
