using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

[DisallowMultipleComponent]
public class NetworkServiceManager : MonoBehaviour
{

    [SerializeField]
    UnityTransport m_UnityTransport;

    bool m_NetworkManagerInitialized = false;

    async void Start()
    {
#if UNITY_EDITOR
        Application.runInBackground = true;
#endif
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            // 매번 다른 ID로 로그인
            var options = new InitializationOptions();
            string randomProfileId = "test_" + Guid.NewGuid().ToString("N").Substring(0, 20);
            options.SetProfile(randomProfileId); 
            await UnityServices.InitializeAsync(options);
        }
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("SignInAnonymouslyAsync");
        }
    }

    public async Task<string> InitializeHost(int maxPlayerCount)
    {
        NetworkManager.Singleton.Shutdown();

        // ShutdownInProgress가 true인 동안 기다림
        while (NetworkManager.Singleton.ShutdownInProgress)
        {
            await Task.Yield(); // 한 프레임 양보 (CPU blocking 방지)
        }

        Allocation joinAllocation = await RelayService.Instance.CreateAllocationAsync(maxPlayerCount);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(joinAllocation.AllocationId);
        var port = joinAllocation.RelayServer.Port;
        var ip = joinAllocation.RelayServer.IpV4;
        Debug.Log($"JoinCode : {joinCode}");
        Debug.Log($"Relay IP: {ip}, Port: {port}");
        m_UnityTransport.SetConnectionData("0.0.0.0", 0);
        var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
        m_UnityTransport.SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();
        m_NetworkManagerInitialized = true;
        return joinCode;
    }

    public async Task InitializeClient(string relayJoinCode)
    {
        NetworkManager.Singleton.Shutdown();
        // ShutdownInProgress가 true인 동안 기다림
        while (NetworkManager.Singleton.ShutdownInProgress)
        {
            await Task.Yield(); // 한 프레임 양보 (CPU blocking 방지)
        }

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
        var port = joinAllocation.RelayServer.Port;
        var ip = joinAllocation.RelayServer.IpV4;
        Debug.Log($"Relay IP: {ip}, Port: {port}");
        m_UnityTransport.SetConnectionData("0.0.0.0", 0);
        var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
        m_UnityTransport.SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();

        m_NetworkManagerInitialized = true;
    }

    public void Uninitialize()
    {
        if (m_NetworkManagerInitialized)
        {
            m_NetworkManagerInitialized = false;
            NetworkManager.Singleton.Shutdown();
        }
    }
}