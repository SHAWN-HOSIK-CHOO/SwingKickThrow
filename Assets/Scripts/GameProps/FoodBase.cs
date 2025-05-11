using System;
using CharacterAttributes;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace GameProps
{
    [RequireComponent(typeof(NetworkObject))]
    public abstract class FoodBase : NetworkBehaviour, IDamageable
    {
        [Header("Health")] 
        public NetworkVariable<float> health = new NetworkVariable<float>();
        private float _currentHealth;

        [Space(5)] [Header("Tag ball is generated in range [min, max]")]
        public int minCounts;
        public  int   maxCounts;
        private int _currentBallCounts;

        // 마지막 공격자 추적
        private ulong _lastAttackerId;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
    
            if (IsServer)
            {
                _currentBallCounts = Random.Range(minCounts, maxCounts);
                health.Value       = 30f; 
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            if (IsServer)
            {
                _currentBallCounts = 0;
                health.Value       = 0f;
            }
        }

        public virtual void TakeDamage(float amount)
        {
            if (IsServer)
            {
                ApplyDamage(amount, OwnerClientId); // 직접 호출 시 OwnerClientId 사용
            }
            else
            {
                TakeDamageServerRpc(amount);
            }
        }

        public float GetHealth()
        {
            return health.Value;
        }

        [ServerRpc]
        private void TakeDamageServerRpc(float amount, ServerRpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            ApplyDamage(amount, clientId);
        }

        protected virtual void ApplyDamage(float amount, ulong attackerClientId)
        {
            if (health.Value <= 0f)
            {
                Debug.Log($"Already below zero : {health.Value}");
                return;
            }
            
            health.Value -= amount;
            
            _lastAttackerId = attackerClientId;

            if (health.Value <= 0f)
            {
                health.Value = 0f;
                
                NotifyDestroyedClientRpc(_lastAttackerId, _currentBallCounts);

                NetworkObject.Despawn();
            }
        }

        [ClientRpc]
        private void NotifyDestroyedClientRpc(ulong clientId, int rewardTagBallCount)
        {
            if (!IsServer)
            {
                return;
            }
            
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
            {
                var playerObject = client.PlayerObject;

                if (playerObject != null)
                {
                    // 원하는 컴포넌트 가져오기
                    var stats = playerObject.GetComponent<CharacterStatusNetwork>();
                    if (stats != null)
                    {
                        stats.tagBalls.Value += rewardTagBallCount;
                        Debug.Log($"{clientId} player has {stats.tagBalls.Value} tag balls");
                    }
                }
            }
        }
        
    }

}
