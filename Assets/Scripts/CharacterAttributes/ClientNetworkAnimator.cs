using Unity.Netcode.Components;

namespace CharacterAttributes
{
   public class ClientNetworkAnimator : NetworkAnimator
   {
      protected override bool OnIsServerAuthoritative()
      {
         return false;
      }
   }
}
