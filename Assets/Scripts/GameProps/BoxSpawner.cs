using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

namespace GameProps
{
    public class BoxSpawner : NetworkBehaviour
    {
        public GameObject pfBox;
        public List<Transform> spawnTransforms;

        private IObjectPool<BoxObject> _boxPool;
        private HashSet<int>           _placedTransforms;

        public int maxBoxCount = 10;
        public int stackDefaultCapacity = 10;

        public IObjectPool<BoxObject> Pool
        {
            get
            {
                if (_boxPool == null)
                {
                    _boxPool = new ObjectPool<BoxObject>(
                        CreatedPoolItem,
                        OnTakeFromPool,
                        OnReturnedToPool,
                        OnDestroyPoolObject,
                        true,
                        stackDefaultCapacity,
                        maxBoxCount
                    );
                }

                return _boxPool;
            }
        }

        public void Initialize(List<Transform> spawnTrans, GameObject pfBox_)
        {
            spawnTransforms   = spawnTrans;
            pfBox             = pfBox_;
            _placedTransforms = new HashSet<int>();
        }

        private BoxObject CreatedPoolItem()
        {
            var go = Instantiate(pfBox);

            if (go == null)
            {
                Debug.LogError("[CreatedPoolItem] Instantiated prefab is null!");
                return null;
            }

            var boxObject = go.GetComponent<BoxObject>();

            if (boxObject == null)
            {
                Debug.LogError("[CreatedPoolItem] BoxObject component missing on instantiated object!");
                Destroy(go); // 혹시라도 남아있을 수 있는 고스트 오브젝트 제거
                return null;
            }

            if (go.GetComponent<NetworkObject>() == null)
            {
                Debug.LogError("[CreatedPoolItem] NetworkObject component missing on instantiated object!");
            }

            boxObject.Pool = Pool;
            boxObject.ResetHealth();

            return boxObject;
        }


        private void OnTakeFromPool(BoxObject boxObject)
        {
            if (boxObject == null)
            {
                Debug.LogError("[Pool] OnTakeFromPool: BoxObject is null!");
                return;
            }

            if (boxObject.NetworkObject == null)
            {
                Debug.LogError("[Pool] OnTakeFromPool: NetworkObject is missing!");
                return;
            }

            if (!boxObject.NetworkObject.IsSpawned)
            {
                boxObject.NetworkObject.Spawn(true);
                Debug.Log($"[Pool] Spawned BoxObject (NetId: {boxObject.NetworkObject.NetworkObjectId})");
            }

            boxObject.ResetHealth();
            //boxObject.EnableClientRpc();
        }

        private void OnReturnedToPool(BoxObject boxObject)
        {
            if (boxObject == null)
            {
                Debug.LogWarning("[Pool] OnReturnedToPool: BoxObject is null!");
                return;
            }

            _placedTransforms.Remove(boxObject.positionNumber);
            boxObject.DisableClientRpc();
        }

        private void OnDestroyPoolObject(BoxObject boxObject)
        {
            if (boxObject == null)
            {
                Debug.LogWarning("[Pool] OnDestroyPoolObject: BoxObject is null!");
                return;
            }

            if (boxObject.NetworkObject != null && boxObject.NetworkObject.IsSpawned)
            {
                boxObject.NetworkObject.Despawn();
                Debug.Log($"[Pool] DestroyPool: Despawned BoxObject (NetId: {boxObject.NetworkObject.NetworkObjectId})");
            }
        }

        public void SpawnBox()
        {
            if (Pool == null)
            {
                Debug.LogError("[Spawner] Pool is null!");
                return;
            }

            if (_placedTransforms.Count >= 5)
            {
                Debug.Log("Max placeable box count is five");
                return;
            }

            int rand = Random.Range(0, spawnTransforms.Count);

            if (_placedTransforms.Contains(rand))
            {
                SpawnBox();
            }
            else
            {
                var box = Pool.Get();

                if (box == null)
                {
                    Debug.LogError("[Spawner] SpawnBox: Got null from Pool.Get()!");
                    return;
                }
                
                Debug.Log($"[Spawner] Spawning Box at {spawnTransforms[rand].position}");

                box.transform.position = spawnTransforms[rand].position;
                box.transform.rotation = spawnTransforms[rand].rotation;
                box.positionNumber     = rand;
                box.EnableClientRpc();
                
                _placedTransforms.Add(rand);
                
                SpawnBox();
            }
            
        }
    }
}
