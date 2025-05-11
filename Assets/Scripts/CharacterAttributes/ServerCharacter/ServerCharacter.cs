using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterAttributes.ServerCharacter
{
    public class ServerCharacter : NetworkBehaviour
    {
        public CharacterControllerNetwork Owner;
        
        public void Initialize(CharacterControllerNetwork owner)
        {
            this.Owner = owner;
        }

        [ServerRpc]
        public void SetPlayerVisibleServerRPC(bool isVisible)
        {
            SetPlayerVisibleClientRPC(isVisible);
        }
        
        [ClientRpc]
        private void SetPlayerVisibleClientRPC(bool isVisible)
        {
            foreach (var myRenderer in GetComponentsInChildren<Renderer>())
                myRenderer.enabled = isVisible;

            foreach (var myCollider in GetComponentsInChildren<Collider>())
                myCollider.enabled = isVisible;
        }
        
        
    }
}
