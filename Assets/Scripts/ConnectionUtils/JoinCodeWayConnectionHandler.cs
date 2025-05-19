using GameFlow;
using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ConnectionUtils
{
    public class JoinCodeWayConnectionHandler : MonoBehaviour
    {
        public Button         requestRelayButton;
        public Button         joinRelayButton;
        public TMP_InputField joinCodeInputField;
        public TMP_Text       joinCodeShowText;

        public GameObject backgroundPanel;
        
        private bool _hasLoadedMainScene = false;
        private bool _hasSpawnCalled     = false;
        
        async void Start()
        {
            backgroundPanel.gameObject.SetActive(true);
#if UNITY_EDITOR
            Application.runInBackground = true;
#endif

            if (NetworkManager.Singleton == null)
            {
                Debug.Log("NetworkManager is null");
            }
            else
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientsConnected;
                NetworkManager.Singleton.OnClientStarted += OnClientStarted;
            }
            
            requestRelayButton.onClick.AddListener(Callback_btn_CreateRelay);
            joinRelayButton.onClick.AddListener(Callback_btn_JoinRelay);
            
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

        }

        private void OnClientStarted()
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManager_OnLoadComplete;
        }

        public async void Callback_btn_CreateRelay()
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(6);
            string     joinCode   = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            joinCodeShowText.text = joinCode;
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
        }
        
        public async void Callback_btn_JoinRelay()
        {
            string userInput = joinCodeInputField.text;
            var joinAllocation  = await RelayService.Instance.JoinAllocationAsync(userInput);
            var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }

        public void OnClientsConnected(ulong clientID)
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                backgroundPanel.gameObject.SetActive(false);
                
                if (!_hasLoadedMainScene && NetworkManager.Singleton.IsHost)
                {
                    _hasLoadedMainScene = true;

                   NetworkManager.Singleton.SceneManager.LoadScene("Game_Main_Flow_Additive", LoadSceneMode.Additive);
                }

                /* bug fix by hp
                    ngo는 LoadScene이 비동기적으로 진행되기 때문에 
                    아직 씬이 로드되지 않은 상태에서 플레이어가 스폰되는 경우 땅으로 떨어지는 경우가 발생            
                    따라서 씬로드 콜백 이후 게임 로직을 실행해야함
                */
            }
        }

        private void SceneManager_OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (sceneName == "Game_Main_Flow_Additive")
            {
                if (!_hasSpawnCalled)
                {
                    _hasSpawnCalled = true;
                    // foreach (var kvp in NetworkManager.Singleton.ConnectedClientsList)
                    // {
                    //     ulong id = kvp.ClientId;
                    //     GameSceneManager.Instance.SpawnMe(id);
                    // }
                    GameSceneManager.Instance.LateInitialize();
                    GameSceneManager.Instance.gameSceneFlowContext.EnterState(GameStateType.GAME_START);
                }
            }
        }

    }
}
