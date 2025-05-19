using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Canvas GameCanvas;
    public Canvas MainCanvas;
    public GameObject DashCharge;
    [Header("Timer")]
    public TMP_Text timeText;
    [Header("Winner")]
    public GameObject winnerUI;
    public Button exit;
}
