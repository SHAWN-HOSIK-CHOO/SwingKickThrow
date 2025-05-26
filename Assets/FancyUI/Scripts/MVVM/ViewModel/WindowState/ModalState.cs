using STC;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
namespace MVVM.ViewModel.Window.State
{
    public class ModalState : STC.State
    {
        IWindowViewModel Owner;
        public ModalState(IWindowViewModel owner, string name = "Modal") : base(name)
        {
            Owner = owner;
        }
        public override void OnSetUp()
        {
            var modalWindow = Owner.ModalWindow as BIND.IViewEventBinding;
            StateChart.Instance.PublishEvent(new STC.StateChart.TaskInEvent()
            {
                task = modalWindow.VisibleState,
                condition = new STC.Condition.AlwaysTrue(),
                ReDo = false,
                ForceActivatePath = true
            });
        }
        public override void OnEnter()
        {
            foreach (var w in Owner.ChildWindow)
            {
                var chlidWindow = w as BIND.IViewEventBinding;
                StateChart.Instance.PublishEvent(new STC.StateChart.TaskInEvent()
                {
                    task = chlidWindow.NonInteractiveState,
                    condition = new STC.Condition.AlwaysTrue(),
                    ReDo = false,
                    ForceActivatePath = true
                });
            }
        }
        public override void OnExit()
        {
            foreach (var w in Owner.ChildWindow)
            {
                var chlidWindow = w as BIND.IViewEventBinding;
                StateChart.Instance.PublishEvent(new STC.StateChart.TaskInEvent()
                {
                    task = chlidWindow.InteractiveState,
                    condition = new STC.Condition.AlwaysTrue(),
                    ReDo = false,
                    ForceActivatePath = true
                });
            }
        }
        public override void OnFinish()
        {
            var modalWindow = Owner.ModalWindow as BIND.IViewEventBinding;
            StateChart.Instance.PublishEvent(new STC.StateChart.TaskInEvent()
            {
                task = modalWindow.HiddenState,
                condition = new STC.Condition.AlwaysTrue(),
                ReDo = false,
                ForceActivatePath = true
            });
        }
    }
}