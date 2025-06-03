using MVVM.Utils;
using MVVM.View.Window;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostSetUpStackView : StackView
{
    public LobbyMenuManager LobbyMenuManager;
    public Window HostSetupWindow;
    public Window HostJoinWindow;
    public Button ConfirmButton;


    public Toggle Private;
    public Toggle TwoPlayer;
    public Toggle ThreePlayer;
    public Toggle FourPlayer;

    public TMP_InputField HostName;
    public TMP_InputField LobbyName;

    private void Start()
    {
        ConfirmButton.onClick.AddListener(OnConfirmButton);
    }
    public override void OnViewStackPushRequest(Stack<StackView> stack)
    {
        stack.Push(this);
        HostSetupWindow.ShowWindow();
        HostJoinWindow.HideWindow();

        LobbyMenuManager.HostName = HostName.text;
        LobbyMenuManager.LobbyName = LobbyName.text;
        LobbyMenuManager.PrivateFlag = Private.isOn;

        if (TwoPlayer.isOn)
        {
            LobbyMenuManager.MaxPlayers = 2;
        }

        if (ThreePlayer.isOn)
        {
            LobbyMenuManager.MaxPlayers = 3;
        }

        if (FourPlayer.isOn)
        {
            LobbyMenuManager.MaxPlayers = 4;
        }
    }

    public async void OnConfirmButton()
    {
        try
        {
            HostSetupWindow.NonInteractable();
            var NetworkServiceManager = NetworkManager.Singleton.GetComponent<NetworkServiceManager>();
            var relayJoinCode = await NetworkServiceManager.InitializeHost(LobbyMenuManager.MaxPlayers);
            if (this == null) return;

            var LobbyManager = NetworkManager.Singleton.GetComponent<LobbyManager>();
            var lobby = await LobbyManager.CreateLobby(LobbyMenuManager.LobbyName,
                LobbyMenuManager.MaxPlayers, LobbyMenuManager.HostName, LobbyMenuManager.PrivateFlag, relayJoinCode);
            if (this == null) return;

            StackViewManager.Instance.Clear();
            SceneManager.LoadScene("Lobby");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public override void OnViewStackPopRequest(Stack<StackView> stack)
    {
        stack.Pop();
        HostSetupWindow.HideWindow();
        HostJoinWindow.ShowWindow();
    }
}
