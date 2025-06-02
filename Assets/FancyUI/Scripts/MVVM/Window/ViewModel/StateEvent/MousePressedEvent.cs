using STC.Event;
using UnityEngine;
public struct MousePressedEvent : STC.Event.IStateEvent
{
    public bool RequireActiveState { get; set; }
    public bool DownStream { get; set; }
    public ConsumeEvent Consumed { get; set; }
    public MousePressedEvent(bool active, bool downStream)
    {
        RequireActiveState = active;
        Consumed = new ConsumeEvent();
        DownStream = downStream;
    }
}