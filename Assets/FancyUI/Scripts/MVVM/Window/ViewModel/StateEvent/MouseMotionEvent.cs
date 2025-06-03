using STC.Event;
using UnityEngine;

namespace MVVM.ViewModel.Window.Event
{
    public struct MouseMotionEvent : STC.Event.IStateEvent
    {
        public bool RequireActiveState { get; set; }
        public bool DownStream { get; set; }
        public ConsumeEvent Consumed { get; set; }
        public MouseMotionEvent( bool active, bool downStream)
        {
            RequireActiveState = active;
            Consumed = new ConsumeEvent();
            DownStream = downStream;
        }
    }
}