// using GameEvents;
// using GameFlow;
// using GameProps.SO;
// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.Serialization;
//
// namespace CharacterAttributes
// {
//     public class CharacterStatusNetwork : NetworkBehaviour, IDamageable, IWelcomer
//     {
//         public float maxHealth;
//         public NetworkVariable<float> currentHealth = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
//         public NetworkVariable<float> defense = new NetworkVariable<float>(5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
//         public NetworkVariable<float> mass = new NetworkVariable<float>();
//
//         [Header("Pushed time after collide")]
//         public NetworkVariable<float> pushedTime = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
//         public NetworkVariable<float> pushedSpeed = new NetworkVariable<float>(20f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
//
//         [Header("Current tag balls")]
//         public NetworkVariable<int> tagBalls = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
//
//         [Header("상대에게 적립시킨 공 개수")]
//         public NetworkVariable<int> scoreBalls = new NetworkVariable<int>();
//         
//         private ulong lastAttackerId = ulong.MaxValue;
//         [Header("스피드 * 질량 : 데미지 ")]
//         public  float crashFactor;
//
//         [FormerlySerializedAs("_speed")]
//         public float speed = 10f;
//
//         public float GetCrashFactor()
//         {
//             crashFactor = speed * mass.Value * 0.05f;
//
//             return crashFactor;
//         }
//         
//         public void Initialize(float initSpeed,float initHealth, float initDefense, float pTime, float pSpeed, int tags, float _mass)
//         {
//             maxHealth = Mathf.Max(initHealth, 1f);
//             speed     = Mathf.Max(initSpeed, 1f);
//             
//             if (!IsOwner)
//                 return;
//
//             InitializeServerRPC(initHealth, initDefense, pTime, pSpeed, tags, _mass);
//         }
//
//         [ServerRpc(RequireOwnership = false)]
//         private void InitializeServerRPC(float initHealth, float initDefense, float pTime, float pSpeed, int tags, float _mass)
//         {
//             maxHealth           = Mathf.Max(initHealth,  1f);
//             currentHealth.Value = Mathf.Max(initHealth,  1f);
//             defense.Value       = Mathf.Max(initDefense, 0f);
//             pushedTime.Value    = pTime;
//             pushedSpeed.Value   = pSpeed;
//             tagBalls.Value      = tags;
//             mass.Value          = _mass;
//             scoreBalls.Value    = 0;
//             
//             if (IsServer)
//             {
//                 currentHealth.OnValueChanged += OnHealthChanged;
//             }
//         }
//         
//         public void MakeCurrentHealthZero()
//         {
//             GameSceneManager.Instance.currentPlayerCrashedUI = GameSceneManager.Instance.splashedScreenUI;
//             MakeCurrentHealthZeroServerRPC();
//         }
//         
//         private bool _isSelfDrown = false;
//         
//         [ServerRpc]
//         private void MakeCurrentHealthZeroServerRPC()
//         {
//             currentHealth.Value = 0f;
//         }
//         
//         private void OnHealthChanged(float previousValue, float newValue)
//         {
//             if (!IsServer) return;
//
//             if (newValue == 0f)
//             {
//                 int lostBalls = Random.Range(0, tagBalls.Value + 1);
//                 tagBalls.Value -= lostBalls;
//
//                 // 2인용이므로 공격자는 나 아닌 한 명뿐
//                 foreach (var client in NetworkManager.Singleton.ConnectedClients)
//                 {
//                     if (client.Key != OwnerClientId) // 나 아닌 상대
//                     {
//                         var attacker = client.Value.PlayerObject.GetComponent<CharacterStatusNetwork>();
//                         if (attacker != null)
//                         {
//                             attacker.scoreBalls.Value += attacker.tagBalls.Value; //현재 가진 공은 모두 적립하고
//                             attacker.tagBalls.Value   =  lostBalls;               //상대 공을 뺏어온다.
//                         }
//                         break;
//                     }
//                 }
//                 
//                 InformDeathClientRPC(OwnerClientId);
//             }
//         }
//
//         [ClientRpc]
//         private void InformDeathClientRPC(ulong deadClientId)
//         {
//             if (NetworkManager.Singleton.LocalClientId != deadClientId)
//             {
//                 Debug.Log("Your enemy is dead");
//                 GameEvents.GameEvents.Publish(GameEventType.ENEMY_DEATH);
//             }
//             else if (NetworkManager.Singleton.LocalClientId == deadClientId)
//             {
//                 Debug.Log("You are dead");
//                 GameEvents.GameEvents.Publish(GameEventType.PLAYER_DEATH);
//                 //물에 빠진 경우에 currentPlayerCrashedUI = splashedScreenUI로 변경하기 때문에 되돌려줘야 함
//                 GameSceneManager.Instance.currentPlayerCrashedUI = GameSceneManager.Instance.playerCrashedScreenUI;
//             }
//         }
//
//         public void TakeDamage(float damage)
//         {
//             if (!IsOwner)
//                 return;
//
//             TakeDamageServerRPC(damage);
//         }
//
//         [ServerRpc(RequireOwnership = false)]
//         private void TakeDamageServerRPC(float damage, ServerRpcParams rpcParams = default)
//         {
//             float damageTaken = Mathf.Max(damage - defense.Value, 1f);
//             currentHealth.Value = Mathf.Max(currentHealth.Value - damageTaken, 0f);
//             lastAttackerId = rpcParams.Receive.SenderClientId; 
//         }
//         
//         public void AddOrMinusTagBalls(int count)
//         {
//             AddOrMinusTagBallsServerRPC(count);
//         }
//
//         [ServerRpc(RequireOwnership = false)]
//         private void AddOrMinusTagBallsServerRPC(int count)
//         {
//             tagBalls.Value += count;
//         }
//
//         public void AddOrMinusDefense(float amount)
//         {
//             AddOrMinusDefenseServerRPC(amount);
//         }
//
//         [ServerRpc(RequireOwnership = false)]
//         private void AddOrMinusDefenseServerRPC(float amount)
//         {
//             if (defense.Value + amount <= 0f)
//             {
//                 defense.Value = 0f;
//             }
//             else
//             {
//                 defense.Value += amount;
//             }
//         }
//         
//         public void AddOrMinusMass(float amount)
//         {
//             AddOrMinusMassServerRPC(amount);
//         }
//
//         [ServerRpc(RequireOwnership = false)]
//         private void AddOrMinusMassServerRPC(float amount)
//         {
//             if (mass.Value + amount <= 30f)
//             {
//                 mass.Value = 30f;
//             }
//             else
//             {
//                 mass.Value += amount;
//             }
//         }
//         
//         public float GetHealth()
//         {
//             return currentHealth.Value;
//         }
//
//         [ServerRpc]
//         public void ResetHealthServerRPC(float hp)
//         {
//             currentHealth.Value = hp;
//         }
//
//         public void Welcome(IVisitor visitor)
//         {
//             visitor.Visit(this);
//         }
//
//         [ClientRpc]
//         public void IncreaseScaleClientRPC(float factor)
//         {
//             //Debug.Log($"client {OwnerClientId} got this message");
//             
//             if (!IsOwner) 
//                 return;
//             
//             if(transform.localScale.magnitude >= new Vector3(2.5f,2.5f,2.5f).magnitude)
//                 return;
//             
//             transform.localScale *= factor;
//         }
//
//         [ClientRpc]
//         public void AddOrMinusSpeedClientRPC(float delta)
//         {
//             if (speed + delta <= 3f)
//             {
//                 speed = 3f;
//             }
//             else
//             {
//                 speed += delta;
//             }
//         }
//
//         [ClientRpc]
//         public void AddOrMinusMaxHealthClientRPC(float delta)
//         {
//             if (maxHealth + delta <= 50f)
//             {
//                 maxHealth = 50f;
//             }
//             else
//             {
//                 maxHealth += delta;
//             }
//         }
//     }
// }
using GameEvents;
using GameFlow;
using GameProps.SO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterAttributes
{
    public class CharacterStatusNetwork : NetworkBehaviour, IDamageable, IWelcomer
    {
        public float maxHealth;
        public NetworkVariable<float> currentHealth = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
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
        public float crashFactor;

        [FormerlySerializedAs("_speed")]
        public float speed = 10f;
        
        private bool _diedBySplash = false;

        public float GetCrashFactor()
        {
            crashFactor = speed * mass.Value * 0.05f;
            return crashFactor;
        }

        public void Initialize(float initSpeed, float initHealth, float initDefense, float pTime, float pSpeed, int tags, float _mass)
        {
            maxHealth = Mathf.Max(initHealth, 1f);
            speed = Mathf.Max(initSpeed, 1f);

            if (!IsOwner)
                return;

            InitializeServerRPC(initHealth, initDefense, pTime, pSpeed, tags, _mass);
        }

        [ServerRpc(RequireOwnership = false)]
        private void InitializeServerRPC(float initHealth, float initDefense, float pTime, float pSpeed, int tags, float _mass)
        {
            maxHealth = Mathf.Max(initHealth, 1f);
            currentHealth.Value = Mathf.Max(initHealth, 1f);
            defense.Value = Mathf.Max(initDefense, 0f);
            pushedTime.Value = pTime;
            pushedSpeed.Value = pSpeed;
            tagBalls.Value = tags;
            mass.Value = _mass;
            scoreBalls.Value = 0;

            if (IsServer)
            {
                currentHealth.OnValueChanged += OnHealthChanged;
            }
        }

        public void MakeCurrentHealthZero()
        {
            _diedBySplash = true;
            GameSceneManager.Instance.currentPlayerCrashedUI = GameSceneManager.Instance.splashedScreenUI;
            MakeCurrentHealthZeroServerRPC();
        }

        [ServerRpc]
        private void MakeCurrentHealthZeroServerRPC()
        {
            currentHealth.Value = 0f;
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
                    if (client.Key != OwnerClientId)
                    {
                        var attacker = client.Value.PlayerObject.GetComponent<CharacterStatusNetwork>();
                        if (attacker != null)
                        {
                            attacker.scoreBalls.Value += attacker.tagBalls.Value;
                            attacker.tagBalls.Value = lostBalls;
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
                
                // if (!_diedBySplash)
                // {
                //     Debug.Log("[UI] Changed to player crash UI");
                //     GameSceneManager.Instance.currentPlayerCrashedUI = GameSceneManager.Instance.playerCrashedScreenUI;
                // }
                //
                // Debug.Log($"[UI] diedBySplash = {_diedBySplash}");
                // _diedBySplash = false;
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
            currentHealth.Value = Mathf.Max(currentHealth.Value - damageTaken, 0f);
            lastAttackerId = rpcParams.Receive.SenderClientId;
        }

        public void AddOrMinusTagBalls(int count)
        {
            AddOrMinusTagBallsServerRPC(count);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddOrMinusTagBallsServerRPC(int count)
        {
            tagBalls.Value += count;
        }

        public void AddOrMinusDefense(float amount)
        {
            AddOrMinusDefenseServerRPC(amount);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddOrMinusDefenseServerRPC(float amount)
        {
            defense.Value = Mathf.Max(defense.Value + amount, 0f);
        }

        public void AddOrMinusMass(float amount)
        {
            AddOrMinusMassServerRPC(amount);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddOrMinusMassServerRPC(float amount)
        {
            mass.Value = Mathf.Max(mass.Value + amount, 30f);
        }

        public float GetHealth()
        {
            return currentHealth.Value;
        }

        [ServerRpc]
        public void ResetHealthServerRPC(float hp)
        {
            currentHealth.Value = hp;
        }

        public void Welcome(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        [ClientRpc]
        public void IncreaseScaleClientRPC(float factor)
        {
            if (!IsOwner)
                return;

            if (transform.localScale.magnitude >= new Vector3(2.5f, 2.5f, 2.5f).magnitude)
                return;

            transform.localScale *= factor;
        }

        [ClientRpc]
        public void AddOrMinusSpeedClientRPC(float delta)
        {
            speed = Mathf.Max(speed + delta, 3f);
        }

        [ClientRpc]
        public void AddOrMinusMaxHealthClientRPC(float delta)
        {
            maxHealth = Mathf.Max(maxHealth + delta, 50f);
        }
    }
}
