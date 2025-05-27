using STC.Event;

namespace MVVM.ViewModel.Window.Event
{
    public struct SetVisibleWindowEvent : STC.Event.IStateEvent
    {
        public bool visible;
        public ConsumeEvent Consumed { get; set; }
        public SetVisibleWindowEvent(bool visible)
        {
            this.visible = visible;
            Consumed = new ConsumeEvent();
        }
    }
}
