using Character;
using FSM;
using UnityEditor.PackageManager;
using UnityEngine;

namespace TickSystem
{
    public class TickController : IOwner<TickController>
    {
        TickController IOwner<TickController>.Get => this;
        IFSM<TickState, TickController> _fsm;
        public NtpClock _ntpClock { get; private set; }
        public double _baseTime { get; private set; }
        public int _renderTick { get; set; }
        public int _simulTick { get; set; }

        void Initialize()
        {
            _ntpClock = new NtpClock(); 
            _fsm = new TickFSM<TickController>(this);
            _baseTime = (_ntpClock.GetNetworkTime() - NtpClock.baseTime).TotalSeconds;
        }

        void Update()
        {
            _fsm.OnUpdate();
        }
    }
}