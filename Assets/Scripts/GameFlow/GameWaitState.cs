using GameFlow;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine.SceneManagement;

public class GameWaitState : BaseGameState
{
    public override void LateSetUp()
    {

    }

    private IEnumerator CheckState()
    {
        while (true)
        {
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                if (GameSceneManager.Instance.playerReady != null )
                {
                    if (GameSceneManager.Instance.playerReady.ReadyList.Count == 2)
                    {
                        bool allReady = true;
                        for (int i = 0; i < GameSceneManager.Instance.playerReady.ReadyList.Count; i++)
                        {
                            allReady &= GameSceneManager.Instance.playerReady.ReadyList[i].IsReady;
                        }
                        if (allReady)
                        {
                            GameSceneManager.Instance.UI.MainCanvas.gameObject.SetActive(false);
                            Finish(GameStateType.Game_WAIT);
                            Context.EnterState(GameStateType.GAME_START);
                            break;
                        }
                    }
                }
            }
            yield return null;
        }
    }

    public override void Handle()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            GameSceneManager.Instance.SpawnPlayerReady();
            GameSceneManager.Instance.SpawnPlayerSelect();
            GameSceneManager.Instance.SpawnTimer();
            NetworkManager.Singleton.SceneManager.LoadScene("Game_Main_Flow_Additive", LoadSceneMode.Additive);
        }
        GameSceneManager.Instance.UI.MainCanvas.gameObject.SetActive(true);
        GameSceneManager.Instance.StartCoroutine(CheckState());
    }

    public override void OnExit()
    {
    }
}
