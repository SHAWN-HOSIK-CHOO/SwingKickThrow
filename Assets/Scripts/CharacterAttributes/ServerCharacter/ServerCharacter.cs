using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterAttributes.ServerCharacter
{
    public class ServerCharacter : NetworkBehaviour
    {
        public enum Trigger
        {
            OnHoverBoardVfx,
            OffHoverBoardVfx,
        }
        public GameObject HoverBoardVfx;
        public CharacterControllerNetwork Owner;
        public void Initialize(CharacterControllerNetwork owner)
        {
            this.Owner = owner;
        }
        public void TriggerTrigger(Trigger trigger)
        {
            if (IsOwner)
            {
                TriggerServerRpc(trigger);
            }
        }
        [ClientRpc]
        void TriggerClientRpc(Trigger trigger)
        {
            switch (trigger)
            {
                case Trigger.OnHoverBoardVfx:
                    HoverBoardVfx.gameObject.SetActive(true);
                    break;
                case Trigger.OffHoverBoardVfx:

                    HoverBoardVfx.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        [ServerRpc]
        void TriggerServerRpc(Trigger trigger)
        {
            TriggerClientRpc(trigger);
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
