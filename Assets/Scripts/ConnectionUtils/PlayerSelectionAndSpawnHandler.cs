using System;
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

        private void Start()
        {
            GameSceneManager.Instance.SetPlayerSelectionHandler(this);
            //TODO: 선택 가능하게 만들기 지금은 일단 0번으로
            //Debug.Log("Start called from spawn handler");
            if (IsOwner)
            {
                //Debug.Log("isowner called from spawn handler");
                SetPlayerSelection(0);
            }
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
            Transform spawnTransform = GameSceneManager.Instance.spawnTransforms[(int)playerID];
            int       selectionInt   = ( (int)playerID == 0 ) ? player0Selection.Value : player1Selection.Value;

            GameObject playerObject = Instantiate(GameSceneManager.Instance.pfPlayers[selectionInt]);
            playerObject.GetComponent<CharacterController>().enabled = false;

            playerObject.transform.position = new Vector3(spawnTransform.position.x,spawnTransform.position.y, spawnTransform.position.z);
            playerObject.transform.rotation = spawnTransform.rotation;

            playerObject.GetComponent<CharacterController>().enabled = true;
            
            playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerID);
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

            Debug.Log("Received request from client : " + clientId);

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
