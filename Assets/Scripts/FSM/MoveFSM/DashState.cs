using FSM;
using UnityEngine;
namespace Character.MoveFSM
{
    public class DashState<OwnerType> : BaseState<MoveState, OwnerType> where OwnerType : MoveController
    { 

    }
}