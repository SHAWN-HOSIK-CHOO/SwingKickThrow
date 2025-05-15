using CharacterAttributes;
using UnityEngine;

namespace GameProps.SO
{
   [CreateAssetMenu(fileName = "Visitor", menuName = "Scriptable Objects/Visitor")]
   public class Visitor : ScriptableObject, IVisitor
   {
      protected float boxHealth = 30f;
      public virtual void Visit(CharacterStatusNetwork stats)
      {
         
      }
   }
}
