using GameFlow;
using System;
using Unity.Collections;
using Unity.Netcode;

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

    private bool _spawnedBoxSpawner;


    public bool SpawnedBoxSpawner
    {
        get => _spawnedBoxSpawner;
        set
        {
            _spawnedBoxSpawner = value;
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
        public FixedString64Bytes ClientId;
        public bool IsReady;

        public bool Equals(PlayerReadyState other) => ClientId.Equals(other.ClientId) && IsReady == other.IsReady;

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
        if (IsHost)
        {
            var LobbyManager = NetworkManager.Singleton.GetComponent<LobbyManager>();
            foreach (var player in LobbyManager.activeLobby.Players)
            {
                var ready = new PlayerReadyState { ClientId = player.Id, IsReady = false };
                ReadyList.Add(ready);
            }

        }
        SetReady();
    }
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SetReadyServerRpc(bool ready, string id, RpcParams rpcParams = default)
    {
        var newState = new PlayerReadyState { ClientId = id, IsReady = ready };
        int index = -1;
        for (int i = 0; i < ReadyList.Count; i++)
        {
            if (ReadyList[i].ClientId == id)
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
        bool isReady = SpawnedTimer & SpawnedPlayerSelect & SpawnedBoxSpawner & _spawnedCharacter;
        var LobbyManager = NetworkManager.Singleton.GetComponent<LobbyManager>();

        SetReadyServerRpc(isReady, LobbyManager.playerId);
    }
}
