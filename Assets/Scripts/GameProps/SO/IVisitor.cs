using CharacterAttributes;
using UnityEngine;

namespace GameProps.SO
{
   public interface IVisitor
   {
      public void Visit(CharacterStatusNetwork stats);
   }
}
