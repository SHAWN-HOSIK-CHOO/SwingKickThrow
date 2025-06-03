using UnityEngine.Events;
using System.Collections.Generic;

namespace GameEvents
{
   public enum GameEventType
   {
      ENEMY_DEATH,
      PLAYER_DEATH,
      PLAYER_CRASH,
      PLAYER_CRASH_END,
      PLAYER_DASH,
      PLAYER_DASH_END,
      PLAYER_RESPAWN,
      PLAYER_DASH_CAHRGED,
      Count,
      GameEnd,
      GameUpdate,
      GameStart,
    }
   
   public class GameEvents
   {
      private static readonly IDictionary<GameEventType, UnityEvent> Events =
         new Dictionary<GameEventType, UnityEvent>();

      public static void Subscribe(GameEventType eventType, UnityAction listener)
      {
         if (Events.TryGetValue(eventType, out var thisEvent))
         {
            thisEvent.AddListener(listener);
         }
         else
         {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Events.Add(eventType,thisEvent);
         }
      }

      public static void Unsubscribe(GameEventType eventType, UnityAction listener)
      {
         if (Events.TryGetValue(eventType, out var thisEvent))
         {
            thisEvent.RemoveListener(listener);
         }
      }

      public static void Publish(GameEventType eventType)
      {
         if (Events.TryGetValue(eventType, out var thisEvent))
         {
            thisEvent.Invoke();
         }
      }
   }
}
