using FSM;
using Unity.Netcode;

namespace Character
{
    public class Controller : NetworkBehaviour , IOwner<Controller>
    {
        IFSM<MoveState,Controller> _fsm;
        public override void OnNetworkSpawn()
        {
            _fsm = new MoveFSM<Controller>(this);
        }

    }
}