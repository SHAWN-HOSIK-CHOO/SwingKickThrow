using MVVM.Property;
using MVVM.ViewModel.Window.State;
using MVVM.ViewModel.Window.Trigger;
namespace MVVM.ViewModel.Window
{
    public class WindowViewModel : STC.State , IWindowState , IWindowTrigger
    {
        public STC.IState InteractiveState { get; private set; }
        public STC.IState NonInteractableState { get; private set; }
        public STC.IState IDLEState { get; private set; }
        public STC.IState TransitionState { get; private set; }
        public STC.IState ModalState { get; private set; }
        public STC.IState VisibleState { get; private set; }
        public STC.IState HiddenState { get; private set; }
        public IWindowViewBinding Binder { get; private set; }

        public BoolProperty VisibleTrigger { get; protected set; }
        public BoolProperty InteractableTrigger { get; protected set; }
        public BoolProperty ModalTrigger { get; protected set; }
        public BoolProperty TransitionTrigger { get; protected set; }

        
        
        
        public WindowViewModel(string name, bool visible, bool interact, bool modal, bool transition) : base(name, StateType.FSM)
        {
            Binder = new WindowViewBinder(this);
            VisibleTrigger = new BoolProperty();
            InteractableTrigger = new BoolProperty();
            InteractableTrigger.Subscribe((val) =>
            {
                if(val)
                {
                    ModalTrigger.SetSilently(false);
                    TransitionTrigger.SetSilently(false);
                }
            });

            ModalTrigger = new BoolProperty();
            ModalTrigger.Subscribe((val) =>
             {
                 InteractableTrigger.SetSilently(!(ModalTrigger.Value || TransitionTrigger.Value));
             });
            TransitionTrigger = new BoolProperty();
            TransitionTrigger.Subscribe((val) =>
            {
                 InteractableTrigger.SetSilently(!(ModalTrigger.Value || TransitionTrigger.Value));
            });

            VisibleTrigger.Value = visible;
            InteractableTrigger.Value = interact;
            ModalTrigger.Value = modal;
            TransitionTrigger.Value = transition;

            VisibleState = new State.VisibleState(this);
            HiddenState = new State.HiddenState(this); 
            InteractiveState = new State.InteractableState(this);
            NonInteractableState = new State.NonInteractableState(this);
            IDLEState = new State.IDLEState(this);
            TransitionState = new TransitionState(this);
            ModalState = new State.ModalState(this);

            PublishAddChildState(this, VisibleState);
            PublishAddChildState(this, HiddenState);
            PublishAddChildState(VisibleState, InteractiveState);
            PublishAddChildState(VisibleState, NonInteractableState);
            PublishAddChildState(InteractiveState, IDLEState);
            PublishAddChildState(NonInteractableState, TransitionState);
            PublishAddChildState(NonInteractableState, ModalState);
        }


        private void PublishAddChildState(STC.IState parent, STC.IState chlid)
        {
            STC.StateChart.Instance.PublishEvent(new STC.StateChart.AddChildEvent()
            {
                state = parent,
                child = chlid,
            });
        }
        public override void OnHandle(STC.Event.IStateEvent e)
        {
            if (e is Event.VisibleWindowEvent visibleEvent)
            {
                visibleEvent.Consumed.isConsumed = true;
                VisibleTrigger.Value = visibleEvent.Visible;
            }
        }

        public bool HasParentWindow()
        {
            var window = Binder;
            var parentState = window.WindowHierachy.Get().ParentState;
            return parentState != null;
        }

        public bool IsParentVisible()
        {
            var window = Binder;
            var parentState = window.WindowHierachy.Get().ParentState;
            if (parentState != null && parentState is WindowHierachyState ParentHierarchy)
            {
                return ParentHierarchy.ViewModel.Binder.Visible.Get();
            }
            return false;
        }
        public bool IsModalWindow()
        {
            var window = Binder;
            var parentState = window.WindowHierachy.Get().ParentState;
            if (parentState != null && parentState is WindowHierachyState ParentHierarchy)
            {
                return Equals(ParentHierarchy.ModalHierachy, window.WindowHierachy.Get());
            }
            return false;
        }

        public bool IsParentModal()
        {
            var window = Binder;
            var parentState = window.WindowHierachy.Get().ParentState;
            if (parentState != null && parentState is WindowHierachyState ParentHierarchy)
            {
                return ParentHierarchy.ViewModel.Binder.Modal.Get();
            }
            return false;
        }
    }
}