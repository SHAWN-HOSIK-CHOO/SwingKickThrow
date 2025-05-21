using GameFlow;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWaitState : BaseGameState
{
    public override void LateSetUp()
    {

    }
    private IEnumerator CheckState()
    {
        // playerReady�� null�� �ƴ� ������ ���
        yield return new WaitUntil(() => GameSceneManager.Instance.playerReady != null);

        // ReadyList�� �� ���� ���� ������ ���
        yield return new WaitUntil(() => GameSceneManager.Instance.playerReady.ReadyList.Count == GameSceneManager.Instance.NumPlayer);

        // �� �� ��� IsReady�� true�� ������ ���
        yield return new WaitUntil(() => {
            bool allReady = true;
            for (int i = 0; i < GameSceneManager.Instance.playerReady.ReadyList.Count; i++)
            {
                allReady &= GameSceneManager.Instance.playerReady.ReadyList[i].IsReady;
            }
            return allReady;
        });
        
        // ���� ���� �� ���� ���·� ��ȯ
        GameSceneManager.Instance.UI.MainCanvas.gameObject.SetActive(false);
        Finish(GameStateType.Game_WAIT);
        Context.EnterState(GameStateType.GAME_START);
    }

    private IEnumerator WaitConnect()
    {        
        // playerReady�� null�� �ƴ� ������ ���
        yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClientsList.Count == GameSceneManager.Instance.NumPlayer);
        if (NetworkManager.Singleton.IsHost)
        {
            GameSceneManager.Instance.SpawnPlayerReady();
            GameSceneManager.Instance.SpawnPlayerSelect();
            GameSceneManager.Instance.SpawnTimer();
            NetworkManager.Singleton.SceneManager.LoadScene("Game_Main_Flow_Additive", LoadSceneMode.Additive);
            foreach (var kvp in NetworkManager.Singleton.ConnectedClientsList)
            {
                ulong id = kvp.ClientId;
                GameSceneManager.Instance.SpawnMe(id);
            }
        }
        GameSceneManager.Instance.StartCoroutine(CheckState());
    }
    public override void Handle()
    {
        GameSceneManager.Instance.StartCoroutine(WaitConnect());
        GameSceneManager.Instance.UI.MainCanvas.gameObject.SetActive(true);
    }

    public override void OnExit()
    {
    }
}
