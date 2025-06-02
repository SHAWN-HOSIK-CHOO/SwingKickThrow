using MVVM.Utils;
using MVVM.View.Window;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostSetUpStackView : StackView
{
    public LobbyMenuManager LobbySceneManager;
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

        LobbySceneManager.HostName = HostName.text;
        LobbySceneManager.LobbyName = LobbyName.text;
        LobbySceneManager.PrivateFlag = Private.isOn;

        if (TwoPlayer.isOn)
        {
            LobbySceneManager.MaxPlayers = 2;
        }

        if (ThreePlayer.isOn)
        {
            LobbySceneManager.MaxPlayers = 3;
        }

        if (FourPlayer.isOn)
        {
            LobbySceneManager.MaxPlayers = 4;
        }
    }

    public async void OnConfirmButton()
    {
        try
        {
            HostSetupWindow.SetInteractable(false);
            var relayJoinCode = await NetworkServiceManager.instance.InitializeHost(LobbySceneManager.MaxPlayers);
            if (this == null) return;

            var lobby = await LobbyManager.instance.CreateLobby(LobbySceneManager.LobbyName,
                LobbySceneManager.MaxPlayers, LobbySceneManager.HostName, LobbySceneManager.PrivateFlag, relayJoinCode);
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
