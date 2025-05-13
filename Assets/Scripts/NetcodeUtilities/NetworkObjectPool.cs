// NetworkObjectPool.cs

using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace NetcodeUtilities
{
    /// <summary>
    /// A basic pool implementation for NetworkObjects, so that object reuse is possible.
    /// </summary>
    public class NetworkObjectPool : MonoBehaviour
    {
        public static NetworkObjectPool Singleton { get; private set; }

        private readonly Dictionary<GameObject, Queue<NetworkObject>> m_PooledObjects = new();
        private readonly Dictionary<NetworkObject, GameObject> m_InstanceToPrefab = new();

        [Tooltip("Optional: Prewarm object count per prefab")]
        public int PrewarmCount = 0;

        private void Awake()
        {
            if (Singleton != null && Singleton != this)
            {
                Debug.LogWarning("Multiple NetworkObjectPool singletons detected, destroying extra.");
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }

        /// <summary>
        /// Registers a prefab to be pooled.
        /// </summary>
        public void RegisterPrefab(GameObject prefab, int prewarmCount = 0)
        {
            if (m_PooledObjects.ContainsKey(prefab)) return;

            var queue = new Queue<NetworkObject>();
            m_PooledObjects[prefab] = queue;

            if (prewarmCount > 0)
            {
                for (int i = 0; i < prewarmCount; i++)
                {
                    var obj = Instantiate(prefab).GetComponent<NetworkObject>();
                    obj.gameObject.SetActive(false);
                    queue.Enqueue(obj);
                    m_InstanceToPrefab[obj] = prefab;
                }
            }
        }

        /// <summary>
        /// Gets a NetworkObject from the pool or instantiates a new one.
        /// </summary>
        public NetworkObject GetNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!m_PooledObjects.TryGetValue(prefab, out var queue))
            {
                RegisterPrefab(prefab, PrewarmCount);
                queue = m_PooledObjects[prefab];
            }

            NetworkObject networkObject;
            if (queue.Count > 0)
            {
                networkObject = queue.Dequeue();
            }
            else
            {
                networkObject = Instantiate(prefab).GetComponent<NetworkObject>();
            }

            var objTransform = networkObject.transform;
            objTransform.SetPositionAndRotation(position, rotation);
            networkObject.gameObject.SetActive(true);
            m_InstanceToPrefab[networkObject] = prefab;

            return networkObject;
        }

        /// <summary>
        /// Returns a NetworkObject back into the pool.
        /// </summary>
        public void ReturnNetworkObject(NetworkObject networkObject)
        {
            if (networkObject == null) return;

            networkObject.gameObject.SetActive(false);

            if (!m_InstanceToPrefab.TryGetValue(networkObject, out var prefab))
            {
                Debug.LogWarning($"[NetworkObjectPool] Tried to return object {networkObject.name} that wasn't created by pool.");
                Destroy(networkObject.gameObject);
                return;
            }

            m_PooledObjects[prefab].Enqueue(networkObject);
        }
    }
}

