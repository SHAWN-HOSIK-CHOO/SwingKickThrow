using STC.Event;

namespace MVVM.ViewModel.Window.Event
{
    public struct SetVisibleWindowEvent : STC.Event.IStateEvent
    {
        public bool visible;
        public bool DownStream { get; set; }
        public ConsumeEvent Consumed { get; set; }
        public SetVisibleWindowEvent(bool visible, bool downStream)
        {
            this.visible = visible;
            Consumed = new ConsumeEvent();

            DownStream = downStream;
        }
    }
}
