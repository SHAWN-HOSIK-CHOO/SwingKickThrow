using MVVM.Utils;
using MVVM.View.Window;
using System.Collections.Generic;
using UnityEngine.UI;

public class LobbyStackView : MVVM.Utils.StackView
{
    public Button backButton;

    public Window HostJoinWindow;
    public Button hostButton;
    public Button joinButton;

    public StackView HostSetup;

    private void Start()
    {
        hostButton.onClick.AddListener(OnHostButton);
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
