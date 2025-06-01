using STC.Event;
using UnityEngine;

namespace MVVM.ViewModel.Window.Event
{
    public struct TransitionEvent : STC.Event.IStateEvent
    {
        public bool State;
        public bool RequireActiveState { get; set; }
        public bool DownStream { get; set; }
        public ConsumeEvent Consumed { get; set; }
        public TransitionEvent(bool state, bool active, bool downStream)
        {
            State = state;
            RequireActiveState = active;
            Consumed = new ConsumeEvent();
            DownStream = downStream;
        }
    }
}