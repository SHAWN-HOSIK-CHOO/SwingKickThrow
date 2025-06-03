using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
public class LobbySceneManager : MonoBehaviour
{
    public async Task LeaveLobby()
    {
        try
        {
            var LobbyManager = NetworkManager.Singleton.GetComponent<LobbyManager>();
            if (LobbyManager.isHost)
            {
                await LobbyManager.DeleteAnyActiveLobbyWithNotify();
            }
            else
            {
                await LobbyManager.LeaveJoinedLobby();
            }
            if (this == null) return;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}