using MVVM.ViewModel.Window.Event;
using STC;
using STC.Event;
using System.Reflection;
using UnityEngine;
namespace MVVM.ViewModel.Window.State
{
    public class WindowHierachyState : STC.State
    {
        public const string StateName = "WindowHierachy";
        public WindowViewModel ViewModel;
        public WindowHierachyState ModalHierachy;

        public WindowHierachyState(WindowViewModel viewModel,string name = StateName) : base(name, StateType.Parallel)
        {
            ViewModel = viewModel;
            ViewModel.Binder.WindowHierachyVal.Value = this;
        }

        public override void OnHandle(IStateEvent e)
        {
            if(e is ModalWindowEvent modalWindowEvent)
            {
                modalWindowEvent.Consumed.isConsumed = true;
                ModalHierachy = modalWindowEvent.ModalWindow.WindowHierachy.Get();
            }
        }

        public override void OnUpdate()
        {
            ViewModel.Update();
        }

        public override void OnSetParent(IState state)
        {

        }

        public override void OnAddChlid(IState state)
        {
            AddTransition(new Transition(Name, state.Name));
        }

        public override void OnActivate()
        {
            ViewModel.Activate();
        }

        public override void OnInActivate()
        {
            ViewModel.InActivate();
        }

    }
}