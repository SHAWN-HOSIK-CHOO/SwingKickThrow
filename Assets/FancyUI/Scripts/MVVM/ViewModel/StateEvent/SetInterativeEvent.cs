using STC.Event;
using UnityEngine;
namespace MVVM.ViewModel.Window.Event
{
    public struct SetInterativeEvent : STC.Event.IStateEvent
    {
        public bool interative;
        public ConsumeEvent Consumed { get; set; }
        public SetInterativeEvent(bool interative)
        {
            this.interative = interative;
            Consumed = new ConsumeEvent();
        }
    }
}