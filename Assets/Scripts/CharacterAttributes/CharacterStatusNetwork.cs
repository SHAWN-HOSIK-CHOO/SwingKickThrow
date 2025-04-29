/*
 * Owner에게 자신의 체력 등을 관리할 권리가 있음
 * Listen Server 모델에서 Server인 클라이언트도 믿기 어렵기 때문에 가장 공정한 방법이라 판단함
 * CharacterStatusNetwork의 모든 코드는 Owner가 아니면 기각되어야 함
 */

using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterAttributes
{
   public class CharacterStatusNetwork : NetworkBehaviour, IDamageable
   {
      public NetworkVariable<float> health      = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server );
      public NetworkVariable<float> attackPower = new NetworkVariable<float>(10f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
      public NetworkVariable<float> defense     = new NetworkVariable<float>(5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

      [Header("Pushed time after collide")] 
      public NetworkVariable<float> pushedTime = new NetworkVariable<float>(0.3f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
      public NetworkVariable<float> pushedSpeed = new NetworkVariable<float>(20f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
      
      public void Initialize(float initHealth, float initAttackPower, float initDefense)
      {
         if(!IsOwner)
            return;
         
         InitializeServerRPC(initHealth,initAttackPower, initDefense);
      }

      [ServerRpc]
      private void InitializeServerRPC(float initHealth, float initAttackPower, float initDefense)
      {
         health.Value      = Mathf.Max(initHealth,      1f);
         attackPower.Value = Mathf.Max(initAttackPower, 1f);
         defense.Value     = Mathf.Max(initDefense,     0f);
      }
      
      public void TakeDamage(float damage)
      {
         if(!IsOwner)
            return;
         
         TakeDamageServerRPC(damage);
      }

      [ServerRpc]
      private void TakeDamageServerRPC(float damage)
      {
         float damageTaken = Mathf.Max(damage - defense.Value, 1f);
         health.Value = Mathf.Max(health.Value - damageTaken, 0f);
      }

      public float GetHealth()
      {
         return health.Value;
      }
      
   }
}
