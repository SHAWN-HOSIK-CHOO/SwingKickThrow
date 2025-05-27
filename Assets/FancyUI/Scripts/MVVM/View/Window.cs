using MVVM.Property;
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
        public WindowViewBinder Binder;

        protected EventTrigger eventTrigger;

        [HideInInspector] public Window ParentWindow;
        public List<Window> ChildWindowList;
        public ModalWindow ModalWindow;
        
        //[Inspector - View] Binding
        public bool Interact;
        BoolProperty _Interact;
        IProperty.DelegateSubscribe Interactable;

        protected virtual void Awake()
        {
            BindView();
            Binder = new WindowViewBinder();
            Binder.BindViewModel(gameObject.name + gameObject.GetInstanceID().ToString());
            Binder.BindVisibleAction(OnVisible);
            Binder.BindTransitionAction(OnTransition);
            Binder.BindModalAction(OnModal);
            Binder.BindInteractiveAction(OnInteractable);

            eventTrigger = GetComponent<EventTrigger>();
            EventTriggerHelper.AddTriggerEvent(eventTrigger, EventTriggerType.PointerClick, OnClick);

            foreach (var w in ChildWindowList)
            {
                w.ParentWindow = this;
            }
            if(ModalWindow != null)
            {
                ModalWindow.ParentWindow = this;
            }

            StateChart.Instance.FlushEventList();
        }
        protected virtual void Start()
        {
            InitializeViewModel();
        }
        void InitializeViewModel()
        {
            foreach (var w in ChildWindowList)
            {
                Binder.ViewEventBinding.AddChildWindow(w.Binder.ViewModel);
            }

            if (gameObject.activeSelf)
            {
                Binder.ViewEventBinding.ShowWindow();
            }
            else
            {
                Binder.ViewEventBinding.HideWindow();
            }

            SetInteractable();
        }
        // Bind [ Inspector - view ] 
        private void Update()
        {
            CheckInteractDirty();
        }
        void BindView()
        {
            Interactable = SetInteractable;
            _Interact = new BoolProperty();
            _Interact.Value = Interact;
            _Interact.Subscribe(Interactable);
        }
        void CheckInteractDirty()
        {
            if (_Interact != null)
            {
                _Interact.Value = Interact;
            }
        }

        //Request ViewModel
        private void OnDisable()
        {
            Binder.ViewEventBinding.HideWindow();
        }
        private void OnEnable()
        {
            Binder.ViewEventBinding.ShowWindow();
        }
        void SetInteractable()
        {
            if (_Interact.Value)
            {
                Binder.ViewEventBinding.SetInteractable();
            }
            else
            {
                Binder.ViewEventBinding.SetNonInteractable();
            }
        }

        public void ModalOpenRequest()
        {
            Binder.ViewEventBinding.OpenModal();
        }
        public void ModalCloseRequest()
        {
            Binder.ViewEventBinding.CloseModal();
        }

        // Update View

        private void OnVisible()
        {
            gameObject.SetActive(Binder.ViewModel.Visible.Value);
        }
        private void OnInteractable()
        {
            Interact = Binder.ViewModel.Interactable.Value;
            eventTrigger.enabled = Binder.ViewModel.Interactable.Value;
        }
        private void OnModal()
        {
            gameObject.SetActive(true);
        }
        private void OnTransition()
        {

        }
        private void OnClick(BaseEventData data)
        {
            PointerEventData pointerData = data as PointerEventData;
            Binder.ViewEventBinding.OnClick(pointerData.position);
        }
    }
}