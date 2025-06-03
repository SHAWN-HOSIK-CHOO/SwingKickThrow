using MVVM.Utils;
using MVVM.View.Window;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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
    }

    private void OnHostButton()
    {
        StackViewManager.Instance.Push(HostSetup);
    }

    private void OnJoinButton()
    {
        StackViewManager.Instance.Push(JoinSearch);
    }


    public override void OnViewStackPushRequest(Stack<MVVM.Utils.StackView> stack)
    {
        stack.Push(this);
        HostJoinWindow.ShowWindow();
    }

    public override void OnViewStackPopRequest(Stack<MVVM.Utils.StackView> stack)
    {
        bool Keep = StackViewManager.Instance.StackCount == 1 && StackViewManager.Instance.IsSame(this);
        if(!Keep)
        {
            stack.Pop();
            HostJoinWindow.HideWindow();
        }
    }
}
