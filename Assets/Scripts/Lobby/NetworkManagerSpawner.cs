using Unity.Netcode;
using UnityEngine;

public class NetworkManagerSpawner : MonoBehaviour
{
    public NetworkManager NetworkManagerPrefab;
    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton == null)
        {

            var manager = Instantiate(NetworkManagerPrefab);
        }
    }
}
