using CharacterAttributes;
using Unity.Netcode;
using UnityEngine;

namespace GameProps.SO
{
   [CreateAssetMenu(fileName = "VolumeUpSO", menuName = "Scriptable Objects/VolumeUpSO")]
   public class VolumeUpSO : Visitor
   {
      public float volumeUpScale;
      public int   tagBalls;
      public float massIncrement;
      public float healthIncrement;
      
      [Header("마이너스")]
      public float speedDecrease;
      
      //현재 network transform은 각자 관리하므로 clientRPC로 클라이언트 본인이 스케일을 키워야함
      public override void Visit(CharacterStatusNetwork stats)
      {
         stats.PlayGotItemAudioClientRPC();
         stats.IncreaseScaleClientRPC(volumeUpScale);
         stats.AddOrMinusTagBalls(tagBalls); 
         stats.AddOrMinusMass(massIncrement);
         stats.AddOrMinusSpeedClientRPC(speedDecrease);
         stats.AddOrMinusMaxHealthClientRPC(healthIncrement);
      }

   }
}
