using STC.Event;
using UnityEngine;

namespace MVVM.ViewModel.Window.Event
{
    public struct MouseWheelEvent : STC.Event.IStateEvent
    {
        public bool RequireActiveState { get; set; }
        public bool DownStream { get; set; }
        public ConsumeEvent Consumed { get; set; }
        public MouseWheelEvent(bool active, bool downStream)
        {
            RequireActiveState = active;
            Consumed = new ConsumeEvent();
            DownStream = downStream;
        }
    }
}