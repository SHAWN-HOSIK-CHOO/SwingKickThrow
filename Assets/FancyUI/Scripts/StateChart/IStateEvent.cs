using UnityEngine;

namespace STC.Event
{ 
    public class ConsumeEvent
    {
        public bool isConsumed;
    }
    public interface IStateEvent
    {
        ConsumeEvent Consumed { get; set; }
    }
}