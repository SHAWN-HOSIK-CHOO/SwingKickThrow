using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
public class LobbyInfo : MonoBehaviour
{
    public Toggle toggle;

    public TextMeshProUGUI LobbyName;

    public TextMeshProUGUI HostName;

    public TextMeshProUGUI MaxPlayer;

    public TextMeshProUGUI CurPlayer;

    public Image LockImage;

    JoinSearchStackView JoinSearchStackView;
    public int lobbyIndex { get; private set; }

    public Lobby lobby { get; private set; }

    public void SetData(JoinSearchStackView joinSearchStackView, Lobby lobby, int lobbyIndex, bool isSelected)
    {
        this.JoinSearchStackView = joinSearchStackView;
        this.lobby = lobby;
        this.lobbyIndex = lobbyIndex;
        LobbyName.text = lobby.Name;
        CurPlayer.text = $"{lobby.Players.Count}";
        MaxPlayer.text = $"{lobby.MaxPlayers}";
        SetLockImage(lobby.IsPrivate);
        toggle.isOn  =isSelected;
    }

    void SetLockImage(bool b)
    {
        var color = LockImage.color;
        color.a = b ? 1 : 0;
        LockImage.color = color;
    }

    public void OnToggleChanged(bool b)
    {
        if(b)
        {
            JoinSearchStackView.SetLobbyIndexSelected(lobbyIndex);
        }
    }
}
