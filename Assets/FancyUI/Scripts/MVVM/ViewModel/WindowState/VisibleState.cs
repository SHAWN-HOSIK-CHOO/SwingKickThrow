using MVVM.Property;
using STC;
namespace MVVM.ViewModel.Window.State
{
    public class VisibleState : STC.State
    {

        public const string StateName = "Visible";
        WindowViewModel Owner;
        BoolProperty VisibleStateBarrier;

        public VisibleState(WindowViewModel owner) : base(StateName, StateType.FSM)
        {
            Owner = owner;
            STC.IState OwnerState = Owner;

            VisibleStateBarrier = new BoolProperty();
            VisibleStateBarrier.Value = true;
            //Owner -> Visible
            var transition = new STC.Transition(OwnerState.Name, VisibleState.StateName);
            var barrier = new STC.BoolBarrier(VisibleStateBarrier, true, OwnerState.Name + VisibleState.StateName);
            transition.Condition.AddCondition(barrier);
            OwnerState.AddTransition(transition);

            //Owner.Hidden -> Owner.Visible 
            transition = new STC.Transition(HiddenState.StateName, VisibleState.StateName);
            barrier = new STC.BoolBarrier(VisibleStateBarrier, true, HiddenState.StateName + VisibleState.StateName);
            transition.Condition.AddCondition(barrier);
            OwnerState.AddTransition(transition);
        }
        public override void OnHandle(STC.Event.IStateEvent e)
        {
            if (e is Event.InteratableEvent interactEvent)
            {
                interactEvent.Consumed.isConsumed = true;
                Owner.InteractableTrigger.Value =  interactEvent.interative;
            }
        }

        public override void OnActivate()
        {
            Owner.Binder.VisibleVal.Value = true;
            foreach (var kvp in Owner.Binder.WindowHierachy.Get().GetAllChildState())
            {
                if (kvp.Value is WindowHierachyState child)
                {
                    child.ViewModel.Binder.ShowWindow();
                }
            }
        }
        public override void OnInActivate()
        {
            Owner.Binder.VisibleVal.Value = false;
            foreach (var kvp in Owner.Binder.WindowHierachy.Get().GetAllChildState())
            {
                if (kvp.Value is WindowHierachyState child)
                {
                    child.ViewModel.Binder.HideWindow();
                }
            }
        }

        public override void OnCheckTransitionBarrier(string src)
        {
            VisibleStateBarrier.SetSilently(true);
            var window = Owner.Binder;
            var parentState = window.WindowHierachy.Get().ParentState;

            bool isParentModal = Owner.IsParentModal();
            bool isParentVisible = Owner.IsParentVisible();
            bool isModalWindow = Owner.IsModalWindow();
            bool hasParent = Owner.HasParentWindow();
            if (isParentVisible)
            {
                if (isModalWindow)
                {
                    if (isParentModal)
                    {
                        Owner.VisibleTrigger.Value = true;
                        VisibleStateBarrier.SetSilently(false);
                    }
                }
                else
                {
                    if (Owner.VisibleTrigger.Value)
                    {
                        VisibleStateBarrier.SetSilently(false);
                    }
                }
            }
            else if (!hasParent) 
            {
                if (Owner.VisibleTrigger.Value)
                {
                    VisibleStateBarrier.SetSilently(false);
                }
            }
        }

        public override void OnEnter()
        {

        }


        public override void OnExit()
        {
            VisibleStateBarrier.SetSilently(true);
        }
    }
}