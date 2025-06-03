using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;
public class LobbySceneManager : MonoBehaviour
{
    public async Task LeaveLobby()
    {
        try
        {
            if (LobbyManager.instance.isHost)
            {
                await LobbyManager.instance.DeleteAnyActiveLobbyWithNotify();
            }
            else
            {
                await LobbyManager.instance.LeaveJoinedLobby();
            }
            if (this == null) return;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}