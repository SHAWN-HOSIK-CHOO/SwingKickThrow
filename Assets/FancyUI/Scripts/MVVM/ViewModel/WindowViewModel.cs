using MVVM.ViewModel.Window.BIND;
using MVVM.ViewModel.Window.Event;
using STC;
using System.Collections.Generic;
using UnityEngine;
namespace MVVM.ViewModel.Window
{
    public class WindowViewModel : STC.State , Window.IWindowViewModel, BIND.IViewEventBinding , IWindowState
    {
        public Property.Property<bool> Visible { get; protected set; }
        public Property.Property<bool> Interactable { get; protected set; }
        public Property.Property<bool> IsModal { get; protected set; }
        public Property.Property<bool> IsTransition { get; protected set; }

        public Window.IWindowViewModel ModalWindow { get; private set; }
        public Window.IWindowViewModel ParentWindow { get; private set; }
        public List<Window.IWindowViewModel> ChildWindow { get; private set; }

        public STC.IState InteractiveState { get; private set; }
        public STC.IState NonInteractiveState { get; private set; }
        public STC.IState IDLEState { get; private set; }
        public STC.IState TransitionState { get; private set; }
        public STC.IState ModalState { get; private set; }
        public STC.IState VisibleState { get; private set; }
        public STC.IState HiddenState { get; private set; }

        public WindowViewModel(string name) : base(name, StateType.FSM)
        {
            Visible = new Property.Property<bool>();
            IsModal = new Property.Property<bool>();
            IsTransition = new Property.Property<bool>();
            Interactable = new Property.Property<bool>();
            IsModal.Subscribe(() => { Interactable.Value = !(IsModal.Value || IsTransition.Value);});
            IsTransition.Subscribe(() => { Interactable.Value = !(IsModal.Value || IsTransition.Value); });

            ChildWindow = new List<IWindowViewModel>();
            VisibleState = new State.VisibleState(this);
            HiddenState = new State.HiddenState(this); 
            InteractiveState = new State.InteractiveState(this);
            NonInteractiveState = new State.NonInteractiveState(this);
            IDLEState = new State.IDLEState(this);
            TransitionState = new State.TransitionState(this);
            ModalState = new State.ModalState(this);

            PublishAddChildState(this, VisibleState);
            PublishAddChildState(this, HiddenState);
            PublishAddChildState(VisibleState, InteractiveState);
            PublishAddChildState(VisibleState, NonInteractiveState);
            PublishAddChildState(InteractiveState, IDLEState);
            PublishAddChildState(NonInteractiveState, TransitionState);
            PublishAddChildState(NonInteractiveState, ModalState);

            STC.StateChart.Instance.AddTransition(new Transition(STC.StateChart.Instance.Name, Name));
            PublishAddChildState(STC.StateChart.Instance, this);
        }
        public STC.IState GetState()
        {
            return this;
        }
        private void PublishAddChildState(STC.IState parent, STC.IState chlid)
        {
            STC.StateChart.Instance.PublishEvent(new STC.StateChart.AddChildEvent()
            {
                state = parent,
                child = chlid,
            });
        }

        #region IViewEventBinding

        public void SetModal(IWindowViewModel modal)
        {
            ModalWindow = modal;
        }
        public void ShowWindow()
        {
            Handle(new SetVisibleWindowEvent(true));
        }
        public void HideWindow()
        {
            Handle(new SetVisibleWindowEvent(false));
        }
        public void OpenModal()
        {
            Handle(new SetModalEvent(true));
        }
        public void CloseModal()
        {
            Handle(new SetModalEvent(false));
        }
        public void SetInteractable()
        {
            Handle(new SetInterativeEvent(true));
        }
        public void SetNonInteractable()
        {
            Handle(new SetInterativeEvent(false));
        }
        public void OnClick(Vector2 pos)
        {

        }
        public void SetParentWindow(IWindowViewModel parent)
        {
            if (ParentWindow == parent) return;
            if(ParentWindow != null)
            {
                var w = ParentWindow as IViewEventBinding;
                w.RemoveChildWindow(this);
            }
            ParentWindow = parent;
            if(ParentWindow!=null)
            {
                var w = ParentWindow as IViewEventBinding;
                w.AddChildWindow(this);
            }
        }
        public void RemoveChildWindow(IWindowViewModel child)
        {
            if (!ChildWindow.Contains(child)) return;
            ChildWindow.Remove(child);
            var w = child as IViewEventBinding;
            w.SetParentWindow(null);
        }
        public void AddChildWindow(IWindowViewModel child)
        {
            if (ChildWindow.Contains(child)) return;
            ChildWindow.Add(child);
            var w = child as IViewEventBinding;
            w.SetParentWindow(this);
        }
        #endregion

        public override void OnHandle(STC.Event.IStateEvent e)
        {
            if (e is Event.SetVisibleWindowEvent visibleEvent)
            {
                if(ParentWindow != null)
                {
                    if (ParentWindow.Visible.Value)
                    {
                        Visible.Value = visibleEvent.visible;
                        visibleEvent.Consumed.isConsumed = true;
                    }
                }
                else
                {
                    Visible.Value = visibleEvent.visible;
                    visibleEvent.Consumed.isConsumed = true;
                }
            }
        }

    }
}