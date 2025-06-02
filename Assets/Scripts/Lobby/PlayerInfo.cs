using System;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public Button KickButton;
    public string ID;
    public TMP_Text Name;
    public Image Ready;
    public bool IsReady;

    public void SetReady(bool b)
    {
        IsReady = b;
        Ready.color = IsReady ? Color.green : Color.red;
    }
}
