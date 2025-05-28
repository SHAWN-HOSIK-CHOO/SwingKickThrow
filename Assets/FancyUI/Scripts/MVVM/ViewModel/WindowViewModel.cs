using MVVM.Property;
using MVVM.ViewModel.Window.Event;
using STC;
using UnityEngine.InputSystem.HID;
namespace MVVM.ViewModel.Window
{
    public class WindowViewModel : STC.State , IWindowState
    {
        public STC.IState InteractiveState { get; private set; }
        public BoolProperty InteractableTrigger { get; private set; }
        public STC.IState NonInteractableState { get; private set; }
        public STC.IState IDLEState { get; private set; }
        public STC.IState TransitionState { get; private set; }
        public BoolProperty TransitionTrigger { get; private set; }
        public STC.IState ModalState { get; private set; }
        public BoolProperty ModalTrigger { get; private set; }

        public STC.IState VisibleState { get; private set; }
        public STC.IState HiddenState { get; private set; }
        public BoolProperty VisibleTrigger { get; private set; }
        public IWindowViewBinding Binder { get; private set; }
        public WindowViewModel(string name) : base(name, StateType.FSM)
        {
            Binder = new WindowViewBinder(this);
            ModalTrigger = new BoolProperty();
            VisibleTrigger = new BoolProperty();
            TransitionTrigger = new BoolProperty();
            InteractableTrigger = new BoolProperty();
            ModalTrigger.Subscribe(() => { InteractableTrigger.Value = !(ModalTrigger.Value || TransitionTrigger.Value) && InteractableTrigger.Value;});
            TransitionTrigger.Subscribe(() => { InteractableTrigger.Value = !(ModalTrigger.Value || TransitionTrigger.Value) && InteractableTrigger.Value;});


            VisibleState = new State.VisibleState(this);
            HiddenState = new State.HiddenState(this); 
            InteractiveState = new State.InteractiveState(this);
            NonInteractableState = new State.NonInteractableState(this);
            IDLEState = new State.IDLEState(this);
            TransitionState = new State.TransitionState(this);
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
            if (e is Event.SetVisibleWindowEvent visibleEvent)
            {
                visibleEvent.Consumed.isConsumed = true;
                VisibleTrigger.Value = visibleEvent.visible;   
            }
        }
    }
}