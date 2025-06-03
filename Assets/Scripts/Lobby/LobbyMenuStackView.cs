using MVVM.Utils;
using MVVM.View.Window;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenuStackView : MVVM.Utils.StackView
{
    public Button backButton;
    public Window HostJoinWindow;
    public Button hostButton;
    public Button joinButton;

    public StackView HostSetup;
    public JoinSearchStackView JoinSearch;

    private void Start()
    {
        hostButton.onClick.AddListener(OnHostButton);
        joinButton.onClick.AddListener(OnJoinButton);
        backButton.onClick.AddListener(OnBackButton);
        StackViewManager.Instance.Push(this);
    }

    private void OnBackButton()
    {
        StackViewManager.Instance.Pop();
        UISoundManager._instance.Back.Play();
    }

    private void OnHostButton()
    {
        StackViewManager.Instance.Push(HostSetup);
        UISoundManager._instance.Click.Play();
    }

    private void OnJoinButton()
    {
        StackViewManager.Instance.Push(JoinSearch);
        UISoundManager._instance.Click.Play();
    }


    public override void OnViewStackPushRequest(Stack<MVVM.Utils.StackView> stack)
    {
        stack.Push(this);
        HostJoinWindow.ShowWindow();
    }

    public override void OnViewStackPopRequest(Stack<MVVM.Utils.StackView> stack)
    {
        stack.Pop();
        SceneManager.LoadScene("Game_Intro_");
    }
}
