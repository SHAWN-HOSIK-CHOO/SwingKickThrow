using MVVM.ViewModel.Window;
using MVVM.ViewModel.Window.BIND;
using MVVM.ViewModel.Window.State;
using STC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Profiling.HierarchyFrameDataView;

namespace MVVM.View.Window
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(EventTrigger))]
    public class Window : MonoBehaviour
    {
        protected EventTrigger eventTrigger;
        protected IViewEventBinding viewModel;
        [HideInInspector] public Window ParentWindow;
        public ModalWindow ModalWindow;
        public List<Window> ChildWindowList;
        protected virtual void Awake()
        {
            foreach (var w in ChildWindowList)
            {
                w.ParentWindow = this;
            }
            if(ModalWindow != null)
            {
                ModalWindow.ParentWindow = this;
            }

            eventTrigger = GetComponent<EventTrigger>();
            EventTriggerHelper.AddTriggerEvent(eventTrigger, EventTriggerType.PointerClick, OnClick);
            BindViewModel();
        }
        protected virtual void Start()
        {
            StateChart.Instance.FlushEventList();
            foreach (var w in ChildWindowList)
            {
                viewModel.AddChildWindow(w.viewModel as IWindowViewModel);
                w.viewModel.SetParentWindow(viewModel as IWindowViewModel);
            }
            viewModel.SetModal(ModalWindow?.viewModel as IWindowViewModel);
            viewModel.Visible();
        }

        void BindViewModel()
        {
            WindowViewModel model = new WindowViewModel(gameObject.name + gameObject.GetInstanceID().ToString());
            model.VisibleState.OnStateEnter += OnVisibleState;
            model.HiddenState.OnStateEnter += OnHiddenState;
            model.InteractiveState.OnStateEnter += OnInteractiveState;
            model.NonInteractiveState.OnStateEnter += OnNonInteractiveState;
            viewModel = model;
        }

        private void OnNonInteractiveState(IState state)
        {
            eventTrigger.enabled = false;
        }

        private void OnInteractiveState(IState state)
        {
            eventTrigger.enabled = true;
        }

        private void OnVisibleState(STC.IState state)
        {
            this.gameObject.SetActive(true);
        }
        private void OnHiddenState(STC.IState state)
        {
            this.gameObject.SetActive(false);
        }


        public void ModalOpenRequest()
        {
            viewModel.OpenModal();
        }
        public void ModalCloseRequest()
        {
            viewModel.CloseModal();
        }
        private void OnClick(BaseEventData data)
        {
            PointerEventData pointerData = data as PointerEventData;
            viewModel.OnClick(pointerData.position);
        }
    }
}