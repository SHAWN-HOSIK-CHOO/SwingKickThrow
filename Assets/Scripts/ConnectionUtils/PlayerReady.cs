using GameFlow;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerReady : NetworkBehaviour
{
    private bool _spawnedTimer;
    public bool SpawnedTimer
    {
        get => _spawnedTimer;
        set
        {
            _spawnedTimer = value;
            SetReady();
        }
    }
    private bool _spawnedPlayerSelect;

    public bool SpawnedCharacter
    {
        get => _spawnedCharacter;
        set
        {
            _spawnedCharacter = value;
            SetReady();
        }
    }
    private bool _spawnedCharacter;


    public bool SpawnedPlayerSelect
    {
        get => _spawnedPlayerSelect;
        set
        {
            _spawnedPlayerSelect = value;
            SetReady();
        }
    }
    public struct PlayerReadyState : INetworkSerializable, IEquatable<PlayerReadyState>
    {
        public ulong ClientId;
        public bool IsReady;

        public bool Equals(PlayerReadyState other) => ClientId == other.ClientId && IsReady == other.IsReady;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref IsReady);
        }
    }
    public NetworkList<PlayerReadyState> ReadyList = new NetworkList<PlayerReadyState>();

    public override void OnNetworkSpawn()
    {
        GameSceneManager.Instance.playerReady = this;
        SetReady();
    }

    [Rpc(SendTo.Server,RequireOwnership = false)]
    private void SetReadyServerRpc(bool ready, RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        var newState = new PlayerReadyState { ClientId = clientId, IsReady = ready };
        int index = -1;
        for (int i = 0; i < ReadyList.Count; i++)
        {
            if (ReadyList[i].ClientId == clientId)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
            ReadyList.Add(newState);
        else
            ReadyList[index] = newState;
    }
    public void SetReady()
    {
        bool isReady = SpawnedTimer & SpawnedPlayerSelect & _spawnedCharacter;
        SetReadyServerRpc(isReady);
    }
}
