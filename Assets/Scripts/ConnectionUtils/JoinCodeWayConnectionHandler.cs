using GameFlow;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectionUtils
{
    public class JoinCodeWayConnectionHandler : MonoBehaviour
    {
        public Button         requestRelayButton;
        public Button         joinRelayButton;
        public TMP_InputField joinCodeInputField;
        public TMP_Text       joinCodeShowText;

        async void Start()
        {
#if UNITY_EDITOR
            Application.runInBackground = true;
#endif
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
            NetworkManager.Singleton.OnClientStarted += OnClientStarted;
            GameSceneManager.Instance.gameSceneFlowContext.EnterState(GameStateType.NONE);
        }

        private void OnClientStarted()
        {
            GameSceneManager.Instance.LateInitialize();
            GameSceneManager.Instance.gameSceneFlowContext.EnterState(GameStateType.Game_WAIT);
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

        private void OnDestroy()
        {
            requestRelayButton.onClick.RemoveListener(Callback_btn_CreateRelay);
            joinRelayButton.onClick.RemoveListener(Callback_btn_JoinRelay);
        }
    }
}
