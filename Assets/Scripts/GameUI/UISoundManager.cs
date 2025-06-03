using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class UISoundManager : MonoBehaviour
{
    public AudioSource Click;
    public AudioSource Back;
    public AudioSource OK;
    public AudioSource Notify;

    public static UISoundManager _instance { get; private set; }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
