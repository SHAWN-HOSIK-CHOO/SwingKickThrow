using MVVM.ViewModel.Window.BIND;
using UnityEngine;
namespace MVVM.ViewModel.Window.State
{
    public class VisibleState : STC.State
    {
        public const string StateName = "Visible";
        IWindowViewModel Owner;
        public VisibleState(IWindowViewModel owner) : base(StateName)
        {
            Owner = owner;
        }

        public override void OnHandle(STC.Event.IStateEvent e)
        {
            if (e is Event.SetModalEvent modalEvent)
            {
                IViewEventBinding modalWindow = Owner.ModalWindow as IViewEventBinding;
                IViewEventBinding ownerWindow = Owner as IViewEventBinding;
                if (modalEvent.active)
                {

                    STC.StateChart.Instance.PublishEvent(new STC.StateChart.TaskInEvent()
                    {
                        task = modalWindow.VisibleState,
                        condition = new STC.Condition.AlwaysTrue(),
                        ReDo = false,
                        ForceActivatePath = true
                    });

                    STC.StateChart.Instance.PublishEvent(new STC.StateChart.TaskInEvent()
                    {
                        task = ownerWindow.NonInteractiveState,
                        condition = new STC.Condition.AlwaysTrue(),
                        ReDo = false,
                        ForceActivatePath = true
                    });
                }
                else
                {
                    STC.StateChart.Instance.PublishEvent(new STC.StateChart.TaskInEvent()
                    {
                        task = modalWindow.HiddenState,
                        condition = new STC.Condition.AlwaysTrue(),
                        ReDo = false,
                        ForceActivatePath = true
                    });


                    STC.StateChart.Instance.PublishEvent(new STC.StateChart.TaskInEvent()
                    {
                        task = ownerWindow.InteractiveState,
                        condition = new STC.Condition.AlwaysTrue(),
                        ReDo = false,
                        ForceActivatePath = true
                    });
                }
            }
        }
    }
}