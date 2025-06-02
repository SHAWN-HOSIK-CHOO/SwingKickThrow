using UnityEditor;
using UnityEngine;

public class LobbySceneManager : MonoBehaviour
{
    HostSetUpStackView HostSetUp;

    public int MaxPlayers;
    public bool PrivateFlag;
    public string LobbyName;
    public string HostName;

    public void OnHostSetMaxPlayers(int maxPlayers)
    {
        if (maxPlayers != 2)
        {
            Debug.Log("현재 2인 플레이까지 지원합니다...");
        }
        MaxPlayers = maxPlayers;
    }
    public void OnHostSetLobbyName(string name)
    {
        LobbyName = name;
    }
    public void OnHostSetHostName(string name)
    {
        HostName = name;
    }

    public void OnHostSetPrivateFlag(bool flag)
    {
        PrivateFlag = flag;
    }
}
