using STC.Event;
using UnityEngine;

namespace MVVM.ViewModel.Window.Event
{


    public struct SetModalEvent : STC.Event.IStateEvent
    {
        public bool active;
        public ConsumeEvent Consumed { get; set; }
        public SetModalEvent(bool active)
        {
            this.active = active;
            Consumed = new ConsumeEvent();
        }
    }
}