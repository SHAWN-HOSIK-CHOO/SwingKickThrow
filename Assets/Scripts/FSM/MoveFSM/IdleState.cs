using FSM;
using UnityEngine;
namespace Character.MoveFSM
{
    public class IdleState<OwnerType> : BaseState<MoveState, OwnerType> where OwnerType : MoveController
    {


    }
}