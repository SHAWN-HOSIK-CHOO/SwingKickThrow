using Character;
using FSM;
using UnityEngine;
namespace TickSystem.TickFSM
{
    public class SimulState<OwnerType> : BaseState<TickState, OwnerType> where OwnerType : TickController
    {
        override protected void OnEnter()
        {

        }

        override protected void OnUpdate()
        {

        }
        override protected void OnExit()
        {

        }
    }
}