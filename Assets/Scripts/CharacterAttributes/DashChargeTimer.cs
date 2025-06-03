using System;
using System.Collections;
using GameEvents;
using GameFlow;
using UnityEngine;

namespace CharacterAttributes
{
   public class DashChargeTimer : MonoBehaviour
   {
      private CharacterControllerNetwork Owner;
      private Coroutine                  _coroutine;

      private float _chargedRatio;
      private float _currentCharge;
      private float _maxCharge;

      private bool _isPaused   = false;
      private bool _isCharging = false;

      public bool IsPaused => _isPaused;

      public void Initialize(CharacterControllerNetwork owner, float max)
      {
         Owner      = owner;
         _maxCharge = max;
         
         if(Owner.IsOwner)
         {
            GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_DASH,  ResetCurrentCharge);
            GameEvents.GameEvents.Subscribe(GameEventType.PLAYER_DEATH, ResetCurrentCharge);
         }
      }

      private void OnDisable()
      {
         if(Owner.IsOwner)
         {
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DASH,  ResetCurrentCharge);
            GameEvents.GameEvents.Unsubscribe(GameEventType.PLAYER_DEATH, ResetCurrentCharge);
         }
      }

      public void StartCharging()
      {
         Debug.Log("Start charging called");

         if (_isCharging && _isPaused)
         {
            // Resume from pause
            _isPaused = false;
            return;
         }

         if (_coroutine != null)
            StopCoroutine(_coroutine);

         _isCharging = true;
         _isPaused   = false;
         if(!_isFullyCharged)
            _coroutine  = StartCoroutine(CoCharging());
      }

      public void StopCharging()
      {
         Debug.Log("Stop charging called");

         _isPaused = true;
         // Coroutine continues to run but charging pauses
      }

      private bool _isFullyCharged = false;

      public bool IsDashCharged()
      {
         return _isFullyCharged;
      }

      public void InitDashChargedFlag()
      {
         _isFullyCharged = false;
      }
      
      private IEnumerator CoCharging()
      {
         while (_currentCharge < _maxCharge)
         {
            if (_isPaused)
            {
               yield return null;
               continue;
            }

            _currentCharge += Time.deltaTime;
            _chargedRatio  = Mathf.Clamp01(_currentCharge / _maxCharge);
            yield return null;
         }

         _isFullyCharged = true;
         GameEvents.GameEvents.Publish(GameEventType.PLAYER_DASH_CAHRGED);
         _chargedRatio = 1f;
         
         _isCharging = false;
         _coroutine  = null;
      }

      private void ResetCurrentCharge()
      {
         Debug.Log("Dash charge has been reset");

         _currentCharge = 0f;
         _chargedRatio  = 0f;
         _isPaused      = false;
         _isCharging    = false;

         if (_coroutine != null)
         {
            StopCoroutine(_coroutine);
            _coroutine = null;
         }
      }

      public float GetChargedRatio()
      {
         return _chargedRatio;
      }

      public bool IsDashReady()
      {
         if (!(_currentCharge >= _maxCharge))
         {
            Debug.Log($"Dash is not ready currently : {_currentCharge}");
         }
         
         return _currentCharge >= _maxCharge;
      }
   }
}
