using System;
using CharacterAttributes;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace GameProps
{
   public class BoxObject : NetworkBehaviour
   {
      public IObjectPool<BoxObject> Pool { get; set; }
      
      private const float      MaxHealth = 30f;
      public      float      currentHealth;
      private       ulong      _lastHitBy;

      [Header("어떤 Transform 인덱스에 위치해 있는지")]
      public int positionNumber;

      private void OnDisable()
      {
         ResetHealth();
      }

      public void ResetHealth()
      {
         currentHealth = MaxHealth;
         _lastHitBy     = 0;
      }

      [ServerRpc(RequireOwnership = false)]
      public void TakeDamageServerRpc(float damage, ulong attackerClientId)
      {
         TakeDamage(damage, attackerClientId);
      }
      
      public void TakeDamage(float damage, ulong attackerClientId)
      {
         if (!IsServer) return;

         currentHealth -= damage;
         _lastHitBy     =  attackerClientId;

         if (currentHealth <= 0)
         {
            Debug.Log($"Destroyed by player {attackerClientId}");
            ReturnToPool();
         }
      }

      private void ReturnToPool()
      {
         Pool?.Release(this);
      }

      [ClientRpc]
      public void DisableClientRpc()
      {
         gameObject.SetActive(false);
      }
      
      [ClientRpc]
      public void EnableClientRpc()
      {
         gameObject.SetActive(true);
      }
   }
}