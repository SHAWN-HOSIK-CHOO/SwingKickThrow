using STC.Event;

namespace MVVM.ViewModel.Window.Event
{
    public struct VisibleWindowEvent : STC.Event.IStateEvent
    {
        public bool Visible;
        public bool DownStream { get; set; }
        public bool Active { get; set; }
        public ConsumeEvent Consumed { get; set; }
        public VisibleWindowEvent(bool visible,bool active, bool downStream)
        {
            Visible = visible;
            Consumed = new ConsumeEvent();
            Active = active;
            DownStream = downStream;
        }
    }
}
