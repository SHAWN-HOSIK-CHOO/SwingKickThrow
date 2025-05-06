using Character;
using FSM;

namespace Character.MoveFSM
{
    public class AnyState<OwnerType> : BaseState<MoveState, OwnerType> where OwnerType : MoveController
    {
       
    }
}