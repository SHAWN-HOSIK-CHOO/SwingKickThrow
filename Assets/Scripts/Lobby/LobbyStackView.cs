using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyStackView : MVVM.Utils.StackView
{
    public LobbySceneManager LobbySceneManager;
    public MVVM.View.Window.Window MainWindow;
    public PlayerInfoContainer PlayerInfoContainer;
    public Button LeaveButton;
    public Toggle ReadyToggle;
    public Text ReadyToggleLabel;

    void Start()
    {
        LeaveButton.onClick.AddListener(OnLeaveButton);
        ReadyToggle.onValueChanged.AddListener(OnReadyToggle);
        PlayerInfoContainer.KickEvent += async (id) => await OnKickEvent(id); ;

        OnReadyToggle(ReadyToggle.isOn);
        OnLobbyChanged(LobbyManager.instance.activeLobby, false);
        LobbyManager.OnLobbyChanged += OnLobbyChanged;
        LobbyManager.OnPlayerNotInLobbyEvent += ReturnToMenu;
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene("LobbyMenu");
    }
    public void OnLobbyChanged(Lobby updatedLobby, bool isGameReady)
    {
        PlayerInfoContainer.SetPlayers(updatedLobby.Players);
        if (isGameReady && LobbyManager.instance.isHost)
        {
            NetworkManager.Singleton.SceneManager.ActiveSceneSynchronizationEnabled = true;
            NetworkManager.Singleton.SceneManager.LoadScene("Game_Flow_", LoadSceneMode.Single);
        }
    }


    async Task OnKickEvent(string id)
    {
        try
        {
            MainWindow.NonInteractable();
            await LobbyManager.instance.RemovePlayer(id);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            if (this != null)
            {
                MainWindow.Interactable();
            }
        }
    }
    

    public async void OnReadyToggle(bool b)
    {
        try
        {
            ReadyToggleLabel.text = b ? "Cancle" : "Ready";
            MainWindow.NonInteractable();
            PlayerInfoContainer.SetPlayerReady(AuthenticationService.Instance.PlayerId, b);
            await LobbyManager.instance.UpdateReadyState(b);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {

            MainWindow.Interactable();
        }
    }

    public async void OnLeaveButton()
    {
        try
        {        
            MainWindow.NonInteractable();
            await LobbySceneManager.LeaveLobby();
            ReturnToMenu();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {

            MainWindow.Interactable();
        }
    }

    void OnDestroy()
    {
        LobbyManager.OnLobbyChanged -= OnLobbyChanged;
        LobbyManager.OnPlayerNotInLobbyEvent -= ReturnToMenu;
    }
}
