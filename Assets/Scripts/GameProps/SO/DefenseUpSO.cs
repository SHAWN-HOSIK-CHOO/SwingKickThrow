using CharacterAttributes;
using UnityEngine;

namespace GameProps.SO
{
   [CreateAssetMenu(fileName = "DefenseUpSO", menuName = "Scriptable Objects/MassUpSO")]
   public class DefenseUpSO : Visitor
   {
      public float defenseIncrement;

      public int tagBalls;
      
      [Header("마이너스")]
      public float speedDecrease;

      public override void Visit(CharacterStatusNetwork stats)
      {
         stats.AddOrMinusTagBalls(tagBalls);
         stats.AddOrMinusDefense(defenseIncrement);
         stats.AddOrMinusSpeedClientRPC(speedDecrease);
      }
   }
}
