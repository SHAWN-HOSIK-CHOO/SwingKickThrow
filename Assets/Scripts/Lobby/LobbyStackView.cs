using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
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
        ReadyToggle.isOn = false;
        PlayerInfoContainer.KickEvent += async (id) => await OnKickEvent(id);
        LobbyManager.OnLobbyChanged += OnLobbyChanged;
        LobbyManager.OnPlayerNotInLobbyEvent += ReturnToMenu;
        var lobbyManager = NetworkManager.Singleton.GetComponent<LobbyManager>();
        if(lobbyManager.activeLobby != null)
        {
            OnLobbyChanged(lobbyManager.activeLobby, LobbyManager.IsGameReady(lobbyManager.activeLobby));
        }
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene("LobbyMenu");
    }
    public void OnLobbyChanged(Lobby updatedLobby, bool isGameReady)
    {
        PlayerInfoContainer.SetPlayers(updatedLobby.Players);
        var LobbyManager = NetworkManager.Singleton.GetComponent<LobbyManager>();
        if (isGameReady && LobbyManager.isHost)
        {
            UISoundManager._instance.Notify.Play();
            NetworkManager.Singleton.SceneManager.LoadScene("Game_Flow_", LoadSceneMode.Single);
        }
    }

    async Task OnKickEvent(string id)
    {
        try
        {
            MainWindow.NonInteractable();

            var LobbyManager = NetworkManager.Singleton.GetComponent<LobbyManager>();
            await LobbyManager.RemovePlayer(id);
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
        if(b)
        {
            UISoundManager._instance.OK.Play();
        }
        else
        {
            UISoundManager._instance.Click.Play();
        }
        try
        {
            ReadyToggleLabel.text = b ? "Cancel" : "Ready";
            MainWindow.NonInteractable();
            PlayerInfoContainer.SetPlayerReady(AuthenticationService.Instance.PlayerId, b);
            var LobbyManager = NetworkManager.Singleton.GetComponent<LobbyManager>();
            await LobbyManager.UpdateReadyState(b);
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
        UISoundManager._instance.Back.Play();
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
