using MVVM.Property;
using MVVM.ViewModel.Window.Event;
using MVVM.ViewModel.Window.State;
using MVVM.ViewModel.Window.Trigger;
using STC;
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
            Binder = new WindowViewBinder(this, visible,interact,modal,transition);
            VisibleTrigger = new BoolProperty();
            VisibleTrigger.Value = Binder.Visible.Get();
            InteractableTrigger = new BoolProperty();
            InteractableTrigger.Value = Binder.Interactable.Get();
            InteractableTrigger.Subscribe(() =>
            {
                if(InteractableTrigger.Value)
                {
                    ModalTrigger.Value = false;
                    TransitionTrigger.Value = false;
                }
            });

            ModalTrigger = new BoolProperty();
            ModalTrigger.Value = Binder.Modal.Get();
            ModalTrigger.Subscribe(() =>
            {
                InteractableTrigger.Value = InteractableTrigger.Value && !ModalTrigger.Get() && !TransitionTrigger.Get();
            });

            TransitionTrigger = new BoolProperty();
            TransitionTrigger.Value = Binder.Transition.Get();
            TransitionTrigger.Subscribe(() =>
            {
                InteractableTrigger.Value = InteractableTrigger.Value && !ModalTrigger.Get() && !TransitionTrigger.Get();
            });


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

            STC.StateChart.Instance.AddTransition(new Transition(STC.StateChart.Instance.Name, Name));
            PublishAddChildState(STC.StateChart.Instance, this);
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
    }
}