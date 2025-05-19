using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameFlow
{
    public class GameTimer : MonoBehaviour
    {
        public float requestedTime;
        float _elapsedTime;
        public float ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;
                OnElapsedTimeChanged?.Invoke(value);
            }
        }
        public Action<float> OnElapsedTimeChanged;

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
            ElapsedTime = 0f;
            while (_elapsedTime <= requestedTime)
            {
                ElapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
