using GameFlow;
using Unity.Netcode;
using UnityEngine;

public class GameNoneState : BaseGameState
{

    public override void LateSetUp()
    {

    }

    public override void Handle()
    {
        GameSceneManager.Instance.UI.MainCanvas.gameObject.SetActive(true);
    }

    public override void OnExit()
    {

    }
}
