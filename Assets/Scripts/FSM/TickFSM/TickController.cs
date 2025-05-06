using Character;
using FSM;
using UnityEngine;

namespace TickSystem
{
    public class TickController : IOwner<TickController>
    {
        TickController IOwner<TickController>.Get => this;
        IFSM<TickState, TickController> _fsm;
        void Initialize()
        {
            _fsm = new TickFSM<TickController>(this);
        }
    }
}