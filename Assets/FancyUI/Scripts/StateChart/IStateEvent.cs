using UnityEngine;

namespace STC.Event
{ 
    public class ConsumeEvent
    {
        public bool isConsumed;
    }
    public interface IStateEvent
    {
        bool DownStream { get; }
        ConsumeEvent Consumed { get; set; }
    }
}