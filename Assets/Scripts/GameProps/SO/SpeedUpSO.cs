using CharacterAttributes;
using UnityEngine;

namespace GameProps.SO
{
   [CreateAssetMenu(fileName = "SpeedUpSO", menuName = "Scriptable Objects/SpeedUpSO")]
   public class SpeedUpSO : Visitor
   {
      public float speedIncrement;

      public int tagBalls;
      
      [Header("마이너스")]
      public float defenseDecrease;
      public float massDecrease;

      public override void Visit(CharacterStatusNetwork stats)
      {
         stats.AddOrMinusTagBalls(tagBalls);
         stats.AddOrMinusSpeedClientRPC(speedIncrement);
         stats.AddOrMinusDefense(defenseDecrease);
         stats.AddOrMinusMass(massDecrease);
      }
   }
}
