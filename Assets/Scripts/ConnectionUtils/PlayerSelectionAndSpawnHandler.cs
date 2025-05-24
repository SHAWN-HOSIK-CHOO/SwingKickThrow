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
            // 플레이어가 두명이라는 가정임 
            int spawnIndex = ((int)playerID == 0) ? 0 : 1;
            Transform spawnTransform = GameSceneManager.Instance.spawnTransforms[spawnIndex];
            int       selectionInt   = ( (int)playerID == 0 ) ? player0Selection.Value : player1Selection.Value;
            
            GameObject playerObject = Instantiate(GameSceneManager.Instance.pfPlayers[selectionInt], new Vector3(spawnTransform.position.x,spawnTransform.position.y, spawnTransform.position.z),spawnTransform.rotation);            
            playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerID);
            _playerTable[(ulong)spawnIndex] = playerObject;
            //Debug.Log($"playerTable of {playerID} null check : {_playerTable[playerID] == null}");
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
