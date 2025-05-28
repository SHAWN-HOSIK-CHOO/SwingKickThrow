using STC.Event;
using UnityEngine;

namespace MVVM.ViewModel.Window.Event
{
    public struct ModalEvent : STC.Event.IStateEvent
    {
        public bool active;
        public bool DownStream { get; set; }
        public ConsumeEvent Consumed { get; set; }
        public ModalEvent(bool active, bool downStream)
        {
            this.active = active;
            Consumed = new ConsumeEvent();
            DownStream = downStream;
        }
    }
}