using STC.Event;
using UnityEngine;

namespace MVVM.ViewModel.Window.Event
{
    public struct ModalWindowEvent : STC.Event.IStateEvent
    {
        public IWindowViewBinding ModalWindow;
        public bool RequireActiveState { get; set; }
        public bool DownStream { get; set; }
        public ConsumeEvent Consumed { get; set; }
        public ModalWindowEvent(IWindowViewBinding modalWindow, bool active, bool downStream)
        {
            ModalWindow = modalWindow;
            RequireActiveState = active;
            Consumed = new ConsumeEvent();
            DownStream = downStream;
        }
    }
}