using System.Collections.Generic;
using CharacterAttributes;
using GameProps.SO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

namespace GameProps
{
    public class BoxObject : NetworkBehaviour
    {
        public IObjectPool<BoxObject> Pool { get; set; }

        private const float MaxHealth = 30f;

        public NetworkVariable<float> currentHealth = new(MaxHealth, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [Header("어떤 Transform 인덱스에 위치해 있는지")]
        public int positionNumber;

        private bool _canIGift = false;
        
        private bool _isReturned = false;
        private bool _isReady = false;
        private ulong _lastHitBy;

        private HashSet<ulong> _alreadyHitPlayers = new();

        [Header("추가 효과")]
        public Visitor[] visitors = new Visitor[3];

        private int _visitorIndex;
        
        [Header("오브젝트")]
        public GameObject[] objects = new GameObject[3];
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _canIGift   = false;
            _isReady    = true;
            _isReturned = false;
            _alreadyHitPlayers.Clear();
            
            if (IsServer)
            {
                ResetEverything();
            }
        }

        public void ResetEverything()
        {
            if (!IsServer) return;

            int rand = Random.Range(0, visitors.Length);
            _visitorIndex = rand;
            Debug.Log($"Visitor index : {_visitorIndex}");

            SetVisitorClientRpc(_visitorIndex);
            
            _canIGift           = false;
            _isReturned         = false;
            _isReady            = true;
            currentHealth.Value = MaxHealth;
            _lastHitBy          = 0;
            _alreadyHitPlayers.Clear();

            //Debug.Log($"[BoxObject] ResetHealth: HP = {currentHealth.Value}, NetId = {NetworkObjectId}");
        }

        [ClientRpc]
        private void SetVisitorClientRpc(int index)
        {
            _visitorIndex = index;

            if (visitors.Length != objects.Length)
            {
                Debug.LogError("Visitors and objects array length mismatch! (client)");
            }

            for (int i = 0; i < visitors.Length; i++)
            {
                objects[i].SetActive(i == _visitorIndex);
            }
        }

        
        public void TakeDamage(float damage, ulong attackerClientId)
        {
            if (!IsServer || !_isReady) return;

            if (currentHealth.Value <= 0)
            {
                Debug.LogWarning($"[BoxObject] Tried to damage already-dead object. NetId: {NetworkObjectId}");
                return;
            }

            currentHealth.Value -= damage;
            _lastHitBy = attackerClientId;

            if (currentHealth.Value <= 0)
            {
                Debug.Log($"[BoxObject] Destroyed by player {attackerClientId}");
                _canIGift = true;
            }
        }

        private void ReturnToPool()
        {
            if (_isReturned)
            {
                Debug.LogWarning($"[BoxObject] Duplicate ReturnToPool call! NetId={NetworkObject?.NetworkObjectId}");
                return;
            }

            _canIGift   = false;
            _isReturned = true;
            _isReady    = false;

            DisableClientRpc(); 
            Pool?.Release(this);

            Debug.Log($"[BoxObject] Returned to pool. NetId={NetworkObject?.NetworkObjectId}");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer || !_isReady) return;

            var player = other.GetComponent<CharacterControllerNetwork>();
            if (player == null) return;

            ulong attackerId = player.OwnerClientId;

            if (_alreadyHitPlayers.Contains(attackerId)) return;
            _alreadyHitPlayers.Add(attackerId);

            Debug.Log($"[BoxObject] Hit by player {attackerId}");
            TakeDamage(100f, attackerId);

            if (_canIGift)
            {
                Debug.Log($"I can gift! to client {player.OwnerClientId}");
                other.GetComponent<CharacterStatusNetwork>().Welcome(visitors[_visitorIndex]);
            }
            
            ReturnToPool();
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
        
        [ClientRpc]
        public void SetPositionClientRpc(Vector3 pos, Quaternion rot)
        {
            transform.SetPositionAndRotation(pos, rot);
        }

    }
}

