using CharacterAttributes;
using Unity.Netcode;
using UnityEngine;

namespace GameProps.SO
{
   [CreateAssetMenu(fileName = "VolumeUpSO", menuName = "Scriptable Objects/VolumeUpSO")]
   public class VolumeUpSO : Visitor
   {
      public int volumeUpScale;
      public int tagBalls;
      
      //현재 network transform은 각자 관리하므로 clientRPC로 클라이언트 본인이 스케일을 키워야함
      public override void Visit(CharacterStatusNetwork stats)
      {
         stats.IncreaseScaleClientRPC(volumeUpScale);
         stats.AddOrMinusTagBalls(tagBalls); // 이건 여전히 ServerRpc로 호출됨
      }

   }
}
