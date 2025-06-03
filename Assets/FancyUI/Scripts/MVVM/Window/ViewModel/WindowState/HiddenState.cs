using MVVM.Property;
using STC;
namespace MVVM.ViewModel.Window.State
{
    public class HiddenState : STC.State
    {
        public const string StateName = "Hidden";
        WindowViewModel Owner;

        BoolProperty HiddenStateBarrier;
        public HiddenState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            IState OwnerState = Owner as IState;
            HiddenStateBarrier = new BoolProperty(nameof(HiddenStateBarrier));
            HiddenStateBarrier.Value = true;
            //Owner -> Hidden
            var transition = new STC.Transition(OwnerState.Name, HiddenState.StateName);
            var barrier = new STC.BoolBarrier(HiddenStateBarrier, true, OwnerState.Name + HiddenState.StateName);
            transition.Condition.AddCondition(barrier);
            
            OwnerState.AddTransition(transition);

            //Owner.Visible -> Owner.Hidden
            transition = new STC.Transition(VisibleState.StateName, HiddenState.StateName);
            barrier = new STC.BoolBarrier(HiddenStateBarrier, true, HiddenState.StateName + HiddenState.StateName);
            transition.Condition.AddCondition(barrier);
            OwnerState.AddTransition(transition);
        }




        public override void OnCheckTransitionBarrier(string src)
        {
            HiddenStateBarrier.SetSilently(true);

            var window = Owner.Binder;
            var parentState = window.WindowHierachy.Get().ParentState;

            bool isParentModal = Owner.IsParentModal();
            bool isParentVisible = Owner.IsParentVisible();
            bool isModalWindow = Owner.IsModalWindow();
            bool hasParent = Owner.HasParentWindow();


            if (!isParentVisible)
            {
                if(hasParent)
                {
                    HiddenStateBarrier.SetSilently(false);
                    
                }
                else
                {
                    if (!Owner.VisibleTrigger.Value)
                    {
                        HiddenStateBarrier.SetSilently(false);
                    }
                }
            }
            else 
            {
                if(isModalWindow)
                {
                    if (!isParentModal)
                    {
                        HiddenStateBarrier.SetSilently(false);
                    }
                }
                else
                {
                    if (!Owner.VisibleTrigger.Value)
                    {
                        HiddenStateBarrier.SetSilently(false);
                    }
                }
            }
        }

        public override void OnActivate()
        {
            Owner.Binder.VisibleVal.Value = false;
        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {
            HiddenStateBarrier.SetSilently(true);
        }

        public override void OnInActivate()
        {

        }
    }
}