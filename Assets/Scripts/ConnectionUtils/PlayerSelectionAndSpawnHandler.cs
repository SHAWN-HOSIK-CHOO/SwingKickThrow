using System;
using System.Collections.Generic;
using GameEvents;
using GameFlow;
using Unity.Netcode;
using UnityEngine;

namespace ConnectionUtils
{
    public class PlayerSelectionAndSpawnHandler : NetworkBehaviour
    {
        public NetworkVariable<int> player0Selection = new NetworkVariable<int>();
        public NetworkVariable<int> player1Selection = new NetworkVariable<int>();
        
        public bool[] hasPlayersConfirmed = new bool[2] { false, false };
        
        public Dictionary<ulong, GameObject> _playerTable = new Dictionary<ulong, GameObject>();
        
        public override void OnNetworkSpawn()
        {
            GameSceneManager.Instance.SetPlayerSelectionHandler(this);
            var player = GameSceneManager.Instance.playerReady;
            player.SpawnedPlayerSelect = true;
        }

        public void SpawnMeWrapper(ulong id)
        {
            if(!IsOwner)
                return;
            
            SpawnPlayerServerRPC(id);
        }
        
        [ServerRpc]
        private void SpawnPlayerServerRPC(ulong playerID)
        {
            if ((int)playerID >= GameSceneManager.Instance.spawnTransforms.Length)
            {
                Debug.Log("Index overflow");
                return;
            }
            //TODO: 인덱스 고치기 만약 id가 1,3 이면 오류 발생
            
            Transform spawnTransform = GameSceneManager.Instance.spawnTransforms[(int)playerID];
            int       selectionInt   = ( (int)playerID == 0 ) ? player0Selection.Value : player1Selection.Value;
            
            GameObject playerObject = Instantiate(GameSceneManager.Instance.pfPlayers[selectionInt], new Vector3(spawnTransform.position.x,spawnTransform.position.y, spawnTransform.position.z),spawnTransform.rotation);
            //playerObject.GetComponent<CharacterController>().enabled = false;

            // playerObject.transform.position = new Vector3(spawnTransform.position.x,spawnTransform.position.y, spawnTransform.position.z);
            // playerObject.transform.rotation = spawnTransform.rotation;
            // 여기서 enable하는거 소용 없음 ServerRPC라 호스트가 아니면 적용 안받음 OnNetworkSpawn()에서 직접 해줘야 함
            //playerObject.GetComponent<CharacterController>().enabled = true;
            //playerObject.GetComponent<CharacterController>().Move(Vector3.zero);
            
            playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerID);
            _playerTable[playerID] = playerObject;
            Debug.Log($"playerTable of {playerID} null check : {_playerTable[playerID] == null}");
        }
        public void SetPlayerSelection(int index)
        {
            if (IsClient)
            {
                SubmitPlayerSelectionServerRpc(index);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SubmitPlayerSelectionServerRpc(int characterIndex, ServerRpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;

            //Debug.Log("Received request from client : " + clientId);

            if (clientId == NetworkManager.Singleton.ConnectedClientsList[0].ClientId)
            {
                player0Selection.Value = characterIndex;
                hasPlayersConfirmed[0]          = true;
            }
            else if (clientId == NetworkManager.Singleton.ConnectedClientsList[1].ClientId)
            {
                player1Selection.Value = characterIndex;
                hasPlayersConfirmed[1]          = true;
            }
            else
            {
                Debug.LogError($"Unexpected ClientId: {clientId}. Unable to assign selection data.");
            }
            
        }
        public int GetLocalPlayerSelection()
        {
            ulong localClientId = NetworkManager.Singleton.LocalClientId;

            if (localClientId == NetworkManager.Singleton.ConnectedClientsList[0].ClientId)
            {
                return player0Selection.Value;
            }
            else
            {
                return player1Selection.Value;
            }
        }
        public int GetEnemySelection()
        {
            ulong localClientId = NetworkManager.Singleton.LocalClientId;

            if (localClientId == NetworkManager.Singleton.ConnectedClientsList[0].ClientId)
            {
                return player1Selection.Value;
            }
            else
            {
                return player0Selection.Value;
            }
        }
    }
}
