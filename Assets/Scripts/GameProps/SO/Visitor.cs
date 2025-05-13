using CharacterAttributes;
using UnityEngine;

namespace GameProps.SO
{
   [CreateAssetMenu(fileName = "Visitor", menuName = "Scriptable Objects/Visitor")]
   public class Visitor : ScriptableObject, IVisitor
   {
      public virtual void Visit(CharacterStatusNetwork stats)
      {
         
      }
   }
}
