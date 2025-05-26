using MVVM.ViewModel.Window.Event;
using System.Collections.Generic;
using UnityEngine;
namespace MVVM.ViewModel.Window
{
    public class WindowViewModel : STC.State , Window.IWindowViewModel, BIND.IViewEventBinding
    {
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

        public WindowViewModel(string name) : base(name)
        {
            ChildWindow = new List<IWindowViewModel>();

            VisibleState = new State.VisibleState(this);
            HiddenState = new State.HiddenState(this); 
            InteractiveState = new State.InteractiveState(this);
            NonInteractiveState = new State.NonInteractiveState(this);
            IDLEState = new State.IDLEState(this);
            TransitionState = new State.TransitionState(this);
            ModalState = new State.ModalState(this);

            AddChildState(this,VisibleState);
            AddChildState(this, HiddenState);
            VisibleState.AddTaskOutMap(HiddenState);
            HiddenState.AddTaskOutMap(VisibleState);

            AddChildState(VisibleState, InteractiveState);
            AddChildState(VisibleState, NonInteractiveState);

            InteractiveState.AddTaskOutMap(NonInteractiveState);
            NonInteractiveState.AddTaskOutMap(InteractiveState);

            AddChildState(InteractiveState, IDLEState);
            AddChildState(NonInteractiveState, TransitionState);
            AddChildState(NonInteractiveState, ModalState);

            AddChildState(STC.StateChart.Instance, this);
            STC.StateChart.Instance.PublishEvent(new STC.StateChart.TaskInEvent()
            {
                task = this,
                condition = new STC.Condition.AlwaysTrue(),
                ReDo = false,
                ForceActivatePath = true,
            });
        }
        public STC.IState GetState()
        {
            return this;
        }
        private void AddChildState(STC.IState parent, STC.IState chlid)
        {
            STC.StateChart.Instance.PublishEvent(new STC.StateChart.AddChildEvent()
            {
                state = parent,
                child = chlid,
                condition = new STC.Condition.AlwaysTrue(),
                ReDo = false
            });
        }
        public void SetModal(IWindowViewModel modal)
        {
            ModalWindow = modal;
        }
        public void OpenModal()
        {
            VisibleState.Handle(new SetModalEvent() { active = true });
        }
        public void CloseModal()
        {
            VisibleState.Handle(new SetModalEvent() { active = false });
        }
        public void Visible()
        {
            STC.StateChart.Instance.PublishEvent(new STC.StateChart.TaskInEvent()
            {
                task = VisibleState,
                condition = new STC.Condition.AlwaysTrue(),
                ReDo = false,
                ForceActivatePath = true,
            });
        }
        public void Hide()
        {
            STC.StateChart.Instance.PublishEvent(new STC.StateChart.TaskInEvent()
            {
                task = HiddenState,
                condition = new STC.Condition.AlwaysTrue(),
                ReDo = false,
                ForceActivatePath = true,
            });
        }
        public void OnClick(Vector2 pos)
        {

        }
        public void SetParentWindow(IWindowViewModel parent)
        {
            ParentWindow = parent;
        }
        public void AddChildWindow(IWindowViewModel child)
        {
            ChildWindow.Add(child);
        }
    }
}