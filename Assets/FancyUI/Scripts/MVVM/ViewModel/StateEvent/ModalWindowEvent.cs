using STC.Event;
using UnityEngine;

namespace MVVM.ViewModel.Window.Event
{
    public struct ModalWindowEvent : STC.Event.IStateEvent
    {
        public IWindowViewBinding ModalWindow;
        public bool ShouldActive { get; set; }
        public bool DownStream { get; set; }
        public ConsumeEvent Consumed { get; set; }
        public ModalWindowEvent(IWindowViewBinding modalWindow, bool active, bool downStream)
        {
            ModalWindow = modalWindow;
            ShouldActive = active;
            Consumed = new ConsumeEvent();
            DownStream = downStream;
        }
    }
}