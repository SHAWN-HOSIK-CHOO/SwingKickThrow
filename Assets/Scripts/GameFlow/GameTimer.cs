using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameFlow
{
    public class GameTimer : NetworkBehaviour
    {
        public NetworkVariable<float> requestedTime = new NetworkVariable<float>( writePerm: NetworkVariableWritePermission.Server);
        public NetworkVariable<float> _elapsedTime = new NetworkVariable<float> (writePerm: NetworkVariableWritePermission.Server);
        private Coroutine _currentCountDownCoroutine;

        public override void OnNetworkSpawn()
        {
            GameSceneManager.Instance.gameTimer = this;
            _elapsedTime.OnValueChanged += GameSceneManager.Instance.UpdateTime;
            var player = GameSceneManager.Instance.playerReady;
            player.SpawnedTimer = true;
        }

        public void StartCountDown()
        {
            if (IsHost)
            {
                if (_currentCountDownCoroutine != null)
                {
                    StopCoroutine(_currentCountDownCoroutine);
                }
                _currentCountDownCoroutine = StartCoroutine(CoStartCountDown());
            }
        }
        
        private IEnumerator CoStartCountDown()
        {
            _elapsedTime.Value = 0f;
            while (_elapsedTime.Value <= requestedTime.Value)
            {
                _elapsedTime.Value += Time.deltaTime;
                yield return null;
            }
        }
    }
}
