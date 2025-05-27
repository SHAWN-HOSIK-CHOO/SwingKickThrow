using MVVM.Property;
using MVVM.ViewModel.Window;
using MVVM.ViewModel.Window.BIND;
using STC;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVVM.View.Window
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(EventTrigger))]
    public class Window : MonoBehaviour
    {
        protected EventTrigger eventTrigger;
        protected IViewEventBinding viewEventBinding;
        protected IWindowViewModel viewModel;


        [HideInInspector] public Window ParentWindow;
        public ModalWindow ModalWindow;
        public List<Window> ChildWindowList;

        IProperty.DelegateSubscribe Visible;
        IProperty.DelegateSubscribe Interactable;
        IProperty.DelegateSubscribe Transition;
        IProperty.DelegateSubscribe Modal;
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
                viewEventBinding.AddChildWindow(w.viewModel);
                w.viewEventBinding.SetParentWindow(viewModel);
            }
            viewEventBinding.SetModal(ModalWindow?.viewModel);

            if (gameObject.activeSelf)
            {
                viewEventBinding.ShowWindow();
            }
            else
            {
                viewEventBinding.HideWindow();
            }
        }

        void BindViewModel()
        {
            WindowViewModel model = new WindowViewModel(gameObject.name + gameObject.GetInstanceID().ToString());
            viewModel = model;
            viewEventBinding = model;

            Visible = OnVisible;
            Interactable = OnInteractable;
            Modal = OnModal;
            Transition = OnTransition;

            viewModel.Visible.Subscribe(Visible);
            viewModel.Interactable.Subscribe(Interactable);
            viewModel.IsModal.Subscribe(Modal);
            viewModel.IsTransition.Subscribe(Transition);
        }
        private void OnVisible()
        {
            this.gameObject.SetActive(viewModel.Visible.Value);
        }
        private void OnInteractable()
        {
            eventTrigger.enabled = viewModel.Interactable.Value;
        }
        private void OnModal()
        {
            this.gameObject.SetActive(true);
        }
        private void OnTransition()
        {

        }

        public void ModalOpenRequest()
        {
            viewEventBinding.OpenModal();
        }
        public void ModalCloseRequest()
        {
            viewEventBinding.CloseModal();
        }

        private void OnClick(BaseEventData data)
        {
            PointerEventData pointerData = data as PointerEventData;
            viewEventBinding.OnClick(pointerData.position);
        }
    }
}