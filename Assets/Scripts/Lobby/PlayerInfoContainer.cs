using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class PlayerInfoContainer : MonoBehaviour
{
    public PlayerInfo PlayerInfoPrefab;

    protected List<PlayerInfo> PlayerInfos;

    public event Action<string> KickEvent;

    private void Awake()
    {
        PlayerInfos = new List<PlayerInfo>();
    }

    void OnKick(string id)
    {
        KickEvent?.Invoke(id);
    }

    public void SetPlayerReady(string id, bool b)
    {
        foreach (var player in PlayerInfos)
        {
            if (player.ID == id)
            {
                player.SetReady(b);
            }
        }
    }

    public virtual void SetPlayers(List<Unity.Services.Lobbies.Models.Player> players)
    {
        RemoveAllPlayers();

        foreach (var player in players)
        {
            AddPlayer(player);
        }
    }


    void AddPlayer(Unity.Services.Lobbies.Models.Player player)
    {
        PlayerInfo playerInfo = GameObject.Instantiate(PlayerInfoPrefab, transform,false);

        var playerId = player.Id;
        var playerName = player.Data[LobbyManager.k_PlayerNameKey].Value;
        var isReady = bool.Parse(player.Data[LobbyManager.k_IsReadyKey].Value);

        playerInfo.ID = playerId;
        playerInfo.Name.text = playerName;
        playerInfo.SetReady(isReady);
        playerInfo.KickButton.onClick.AddListener(() =>
        {
            OnKick(playerInfo.ID);
        });
        PlayerInfos.Add(playerInfo);
    }

    void RemoveAllPlayers()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        PlayerInfos.Clear();
    }
}
