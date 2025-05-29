using UnityEngine;

namespace STC.Event
{ 
    public class ConsumeEvent
    {
        public bool isConsumed;
    }
    public interface IStateEvent
    {
        bool Active { get; }  // 이벤트 전달 조건 상태 
        bool DownStream { get; } // 이벤트의 흐름 방향
        ConsumeEvent Consumed { get; set; } // 이벤트의 소비 여부
    }
}