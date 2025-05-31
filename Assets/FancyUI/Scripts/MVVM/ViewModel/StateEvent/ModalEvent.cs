using STC.Event;
using UnityEngine;

namespace MVVM.ViewModel.Window.Event
{
    public struct ModalEvent : STC.Event.IStateEvent
    {
        public bool Modal;
        public bool RequireActiveState { get; set; }
        public bool DownStream { get; set; }
        public ConsumeEvent Consumed { get; set; }
        public ModalEvent(bool modal,bool active, bool downStream)
        {
            Modal = modal;
            RequireActiveState = active;
            Consumed = new ConsumeEvent();
            DownStream = downStream;
        }
    }
}