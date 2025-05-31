using STC.Event;
using UnityEngine;
namespace MVVM.ViewModel.Window.Event
{
    public struct InteratableEvent : STC.Event.IStateEvent
    {
        public bool interative;
        public bool DownStream { get; set; }
        public bool RequireActiveState { get; set; }
        
        public ConsumeEvent Consumed { get; set; }
        public InteratableEvent(bool interative, bool active, bool downStream)
        {
            this.interative = interative;
            Consumed = new ConsumeEvent();
            DownStream = downStream;
            RequireActiveState = active;
        }
    }
}