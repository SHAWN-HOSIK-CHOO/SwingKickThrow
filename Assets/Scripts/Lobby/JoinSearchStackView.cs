using MVVM.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinSearchStackView : StackView
{
    public MVVM.View.Window.Window HostJoinWindow;
    public MVVM.View.Window.Window JoinSearchWindow;
    public MVVM.View.Window.Window JoinWindow;

    List<Lobby> LobbyList;
    List<LobbyInfo> LobbyInfoList;

    int m_LobbyIndexSelected;
    public ToggleGroup LobbyInfoToggleGroup;
    public Transform LobbyListContainer;
    public LobbyInfo LobbyInfoPrefab;
    public TMP_InputField NickName;
    public TextMeshProUGUI ModalMsg;
    public Button Join;


    float m_UpdateLobbiesListTime = float.MaxValue;
    float k_UpdateLobbiesListInterval = 1.5f;
    enum HideFullFilter
    {
        Any,
        Yes,
    }

    HideFullFilter CurrentHideFullFilter;

    public override void OnViewStackPushRequest(Stack<StackView> stack)
    {
        stack.Push(this);
        JoinSearchWindow.ShowWindow();
        HostJoinWindow.HideWindow();
        OnJoinSearch();
    }

    public async void OnJoinSearch()
    {
        try
        {
            // Clear any old lobby selection so nothing is selected when player first enters the lobby.
            ClearLobbySelection();
            var LobbyManager = NetworkManager.Singleton.GetComponent<LobbyManager>();
            var lobbies = await LobbyManager.GetUpdatedLobbiesList();
            if (this == null) return;
            SetLobbies(lobbies);
            LobbyManager.Log("Showing game select panel", lobbies);
            LobbyListUpdateStart();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            if (this != null)
            {
                JoinWindow.Interactable();
            }
        }
    }



    public override void OnViewStackPopRequest(Stack<StackView> stack)
    {
        stack.Pop();
        JoinSearchWindow.HideWindow();
        HostJoinWindow.ShowWindow();
    }

    void Start()
    {
        LobbyInfoList = new List<LobbyInfo>();
        LobbyList = new List<Lobby>();
        m_LobbyIndexSelected = -1;

        Join.onClick.AddListener(OnJoinLobbyButton);
    }




    public void LobbyListUpdateStart()
    {
        m_UpdateLobbiesListTime = Time.time + k_UpdateLobbiesListInterval;
    }

    public void LobbyListUpdateEnd()
    {
        m_UpdateLobbiesListTime = float.MaxValue;
    }

    async void Update()
    {
        try
        {
            if (Time.time >= m_UpdateLobbiesListTime)
            {
                // Turn off lobbies-list updating. We will only turn it back on if still on lobby select Panel AND
                // we've finished updating the lobbies list.
                LobbyListUpdateEnd();

                // If not showing the game select Panel then turn off updating.
                if (!JoinWindow.Binder.ViewBinding.Visible.Get())
                {
                    return;
                }

                var LobbyManager = NetworkManager.Singleton.GetComponent<LobbyManager>();
                var lobbies = await LobbyManager.GetUpdatedLobbiesList();
                if (this == null) return;
                // Update the lobbies list, clear lobby selection if no longer valid.
                UpdateLobbies(lobbies);
                LobbyListUpdateStart();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }


    public void SetLobbies(List<Lobby> lobbiesList)
    {
        LobbyList = lobbiesList;
        RefreshLobbies();
    }
    public void UpdateLobbies(List<Lobby> lobbiesList)
    {
        if (!DidLobbiesChange(lobbiesList))
        {
            return;
        }

        LobbyManager.Log("Updating Lobbies list for changes", lobbiesList);

        string oldSelectedLobbyId = null;
        if (m_LobbyIndexSelected >= 0 && m_LobbyIndexSelected < LobbyList.Count)
        {
            oldSelectedLobbyId = LobbyList[m_LobbyIndexSelected].Id;
        }

        m_LobbyIndexSelected = -1;
        SetLobbies(lobbiesList);

        if (oldSelectedLobbyId != null)
        {
            for (int lobbyIndex = 0; lobbyIndex < LobbyList.Count; lobbyIndex++)
            {
                if (LobbyList[lobbyIndex].Id == oldSelectedLobbyId)
                {
                    m_LobbyIndexSelected = lobbyIndex;
                    break;
                }
            }
        }

        UpdateLobbySelectionIndex(m_LobbyIndexSelected);
    }
    public void UpdateLobbySelectionIndex(int lobbyIndex)
    {
        m_LobbyIndexSelected = lobbyIndex;

        if (lobbyIndex < 0)
        {
            ClearLobbySelection();
        }
        else
        {
            SetLobbyIndexSelected(lobbyIndex);
        }
    }
    public void SetLobbyIndexSelected(int lobbyIndex)
    {
        ClearLobbySelection();
        m_LobbyIndexSelected = lobbyIndex;
        UpdateJoinButtonInteractability();
    }
    public void ClearLobbySelection()
    {
        m_LobbyIndexSelected = -1;
        JoinWindow.NonInteractable();
    }
    public void OnHideFullFilterChanged(int dropdownSelectionIndex)
    {
        CurrentHideFullFilter = (HideFullFilter)dropdownSelectionIndex;
        RefreshLobbies();
    }
    void RefreshLobbies()
    {
        RemoveAllLobbies();
        JoinWindow.NonInteractable();
        int lobbyIndex = 0;
        foreach (var lobby in LobbyList)
        {
            if (IsFilteredIn(lobby))
            {
                AddLobby(lobby, lobbyIndex);
            }
            else if (lobbyIndex == m_LobbyIndexSelected)
            {
                m_LobbyIndexSelected = -1;
                ClearLobbySelection();
            }

            lobbyIndex++;
        }

        UpdateJoinButtonInteractability();
    }
    bool IsFilteredIn(Lobby lobby)
    {
        return IsLobbyFullFilteredIn(lobby);
    }
    bool IsLobbyFullFilteredIn(Lobby lobby)
    {
        if (CurrentHideFullFilter == HideFullFilter.Yes && lobby.Players.Count >= lobby.MaxPlayers)
        {
            return false;
        }

        return true;
    }
    void UpdateJoinButtonInteractability()
    {
        bool b = m_LobbyIndexSelected >= 0 &&
            m_LobbyIndexSelected < LobbyInfoList.Count;

        if (b)
        {
            JoinWindow.Interactable();
        }
        else
        {
            JoinWindow.NonInteractable();
        }
    }
    void AddLobby(Lobby lobby, int lobbyIndex)
    {
        var lobbyInfo = GameObject.Instantiate(LobbyInfoPrefab, LobbyListContainer,false);
        lobbyInfo.SetData(this, lobby, lobbyIndex, lobbyIndex == m_LobbyIndexSelected);
        lobbyInfo.toggle.group = LobbyInfoToggleGroup;
        LobbyInfoList.Add(lobbyInfo);
    }
    void RemoveAllLobbies()
    {
        for (var i = LobbyListContainer.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(LobbyListContainer.GetChild(i).gameObject);
        }

        LobbyInfoList.Clear();
    }
    bool DidLobbiesChange(List<Lobby> lobbiesList)
    {
        if (lobbiesList.Count != LobbyList.Count)
        {
            return true;
        }

        for (int i = 0; i < lobbiesList.Count; i++)
        {
            if (lobbiesList[i].LobbyCode != LobbyList[i].LobbyCode)
            {
                return true;
            }
        }

        return false;
    }
    
    async Task InitializeRelayClient(Lobby lobbyJoined)
    {
        try
        {
            var relayJoinCode = lobbyJoined.Data[LobbyManager.k_RelayJoinCodeKey].Value;

            var NetworkServiceManager = NetworkManager.Singleton.GetComponent<NetworkServiceManager>();

            await NetworkServiceManager.InitializeClient(relayJoinCode);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    async Task OpenLobby(Lobby lobbyJoined)
    {
        try
        {
            await InitializeRelayClient(lobbyJoined);
            if (this == null) return;

            SceneManager.LoadScene("Lobby");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    public async void OnJoinLobbyButton()
    {
        UISoundManager._instance.OK.Play();
        try
        {
            JoinSearchWindow.NonInteractable();

            var LobbyManager = NetworkManager.Singleton.GetComponent<LobbyManager>();
            var lobbiesList = LobbyManager.lobbiesList;
            if (m_LobbyIndexSelected >= 0 && m_LobbyIndexSelected <= lobbiesList.Count)
            {
                var lobbyToJoin = lobbiesList[m_LobbyIndexSelected];
                var lobbyJoined = await LobbyManager.JoinLobby(lobbyToJoin.Id, NickName.text);
                if (this == null) return;

                // If lobby no longer exists (i.e. host left) then popup
                if (lobbyJoined == null)
                {

                    ModalMsg.text = "LobbyNotFound";
                    JoinSearchWindow.OpenModal();
                }
                else
                {
                    await OpenLobby(lobbyJoined);
                }
            }
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.LobbyNotFound)
        {
            ModalMsg.text = "LobbyNotFound";
            JoinSearchWindow.OpenModal();
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.LobbyFull)
        {
            ModalMsg.text = "Lobby is Full";
            JoinSearchWindow.OpenModal();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            if (this != null)
            {
                JoinSearchWindow.Interactable();
            }
        }
    }
}
