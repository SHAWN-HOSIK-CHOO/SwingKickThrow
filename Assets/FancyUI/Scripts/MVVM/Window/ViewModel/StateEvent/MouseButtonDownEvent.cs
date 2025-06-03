using STC.Event;
using UnityEngine;

namespace MVVM.ViewModel.Window.Event
{
    public struct MouseButtonDownEvent : STC.Event.IStateEvent
    {
        public bool RequireActiveState { get; set; }
        public bool DownStream { get; set; }
        public ConsumeEvent Consumed { get; set; }
        public MouseButtonDownEvent(bool active, bool downStream)
        {
            RequireActiveState = active;
            Consumed = new ConsumeEvent();
            DownStream = downStream;
        }
    }
}
