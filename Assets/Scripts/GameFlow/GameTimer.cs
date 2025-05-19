using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameFlow
{
    public class GameTimer : MonoBehaviour
    {
        public  float requestedTime;
        private float _elapsedTime;

        private Coroutine _currentCountDownCoroutine;

        public void Initialize(float targetTime)
        {
            requestedTime = targetTime;
        }

        public void StartCountDown()
        {
            if (_currentCountDownCoroutine != null)
            {
                StopCoroutine(_currentCountDownCoroutine);
            }

            _currentCountDownCoroutine = StartCoroutine(CoStartCountDown());
        }
        
        private IEnumerator CoStartCountDown()
        {
            _elapsedTime = 0f;

            while (_elapsedTime <= requestedTime)
            {
                _elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
