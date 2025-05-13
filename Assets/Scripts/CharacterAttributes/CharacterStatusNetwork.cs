using GameEvents;
using GameProps.SO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterAttributes
{
    public class CharacterStatusNetwork : NetworkBehaviour, IDamageable, IWelcomer
    {
        public NetworkVariable<float> health = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<float> defense = new NetworkVariable<float>(5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<float> mass = new NetworkVariable<float>();

        [Header("Pushed time after collide")]
        public NetworkVariable<float> pushedTime = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<float> pushedSpeed = new NetworkVariable<float>(20f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [Header("Current tag balls")]
        public NetworkVariable<int> tagBalls = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [Header("상대에게 적립시킨 공 개수")]
        public NetworkVariable<int> scoreBalls = new NetworkVariable<int>();
        
        private ulong lastAttackerId = ulong.MaxValue;
        [Header("스피드 * 질량 : 데미지 ")]
        public  float crashFactor;

        [FormerlySerializedAs("_speed")]
        public float speed = 10f;

        public float GetCrashFactor()
        {
            crashFactor = speed * mass.Value * 0.05f;

            return crashFactor;
        }
        
        public void Initialize(float initHealth, float initDefense, float pTime, float pSpeed, int tags, float _mass)
        {
            if (!IsOwner)
                return;

            InitializeServerRPC(initHealth, initDefense, pTime, pSpeed, tags, _mass);
        }

        [ServerRpc(RequireOwnership = false)]
        private void InitializeServerRPC(float initHealth, float initDefense, float pTime, float pSpeed, int tags, float _mass)
        {
            health.Value      = Mathf.Max(initHealth, 1f);
            defense.Value     = Mathf.Max(initDefense, 0f);
            pushedTime.Value  = pTime;
            pushedSpeed.Value = pSpeed;
            tagBalls.Value    = tags;
            mass.Value        = _mass;
            scoreBalls.Value  = 0;
            
            if (IsServer)
            {
                health.OnValueChanged += OnHealthChanged;
            }
        }

        private void OnHealthChanged(float previousValue, float newValue)
        {
            if (!IsServer) return;

            if (newValue == 0f)
            {
                int lostBalls = Random.Range(0, tagBalls.Value + 1);
                tagBalls.Value -= lostBalls;

                // 2인용이므로 공격자는 나 아닌 한 명뿐
                foreach (var client in NetworkManager.Singleton.ConnectedClients)
                {
                    if (client.Key != OwnerClientId) // 나 아닌 상대
                    {
                        var attacker = client.Value.PlayerObject.GetComponent<CharacterStatusNetwork>();
                        if (attacker != null)
                        {
                            attacker.scoreBalls.Value += attacker.tagBalls.Value; //현재 가진 공은 모두 적립하고
                            attacker.tagBalls.Value   =  lostBalls;               //상대 공을 뺏어온다.
                        }
                        break;
                    }
                }
                
                InformDeathClientRPC(OwnerClientId);
            }
        }

        [ClientRpc]
        private void InformDeathClientRPC(ulong deadClientId)
        {
            if (NetworkManager.Singleton.LocalClientId != deadClientId)
            {
                Debug.Log("Your enemy is dead");
                GameEvents.GameEvents.Publish(GameEventType.ENEMY_DEATH);
            }
            else if (NetworkManager.Singleton.LocalClientId == deadClientId)
            {
                Debug.Log("You are dead");
                GameEvents.GameEvents.Publish(GameEventType.PLAYER_DEATH);
            }
        }

        public void TakeDamage(float damage)
        {
            if (!IsOwner)
                return;

            TakeDamageServerRPC(damage);
        }

        [ServerRpc(RequireOwnership = false)]
        private void TakeDamageServerRPC(float damage, ServerRpcParams rpcParams = default)
        {
            float damageTaken = Mathf.Max(damage - defense.Value, 1f);
            health.Value = Mathf.Max(health.Value - damageTaken, 0f);
            lastAttackerId = rpcParams.Receive.SenderClientId; // 기록해두긴 하지만 안 써도 됨
        }

        //서버에서만 실행되어야 하는 메소드
        public void AddOrMinusTagBalls(int count)
        {
            AddOrMinusTagBallsServerRPC(count);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddOrMinusTagBallsServerRPC(int count)
        {
            tagBalls.Value += count;
        }

        public float GetHealth()
        {
            return health.Value;
        }

        [ServerRpc]
        public void ResetHealthServerRPC(float hp)
        {
            health.Value = hp;
        }

        public void Welcome(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        [ClientRpc]
        public void IncreaseScaleClientRPC(int factor)
        {
            Debug.Log($"client {OwnerClientId} got this message");
            
            if (!IsOwner) 
                return;
            
            if(transform.localScale.magnitude >= new Vector3(4f,4f,4f).magnitude)
                return;
            
            transform.localScale *= factor;
        }



    }
}
