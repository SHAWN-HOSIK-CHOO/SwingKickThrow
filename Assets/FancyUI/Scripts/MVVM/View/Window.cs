using MVVM.Property;
using STC;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVVM.View.Window
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(EventTrigger))]
    public class Window : MonoBehaviour
    {
        public List<Window> ChildWindowList;
        public ModalWindow ModalWindow;
        public GameObject RenderObj;

        [HideInInspector]  public Window ParentWindow;

        [HideInInspector][SerializeField] public bool __visible;
        [HideInInspector][SerializeField] public bool __Interact;
        [HideInInspector][SerializeField] public bool __Modal;
        [HideInInspector][SerializeField] public bool __Transition;

        [HideInInspector]  public BoolProperty Visible;
        [HideInInspector]  public BoolProperty Interact;
        [HideInInspector]  public BoolProperty Transition;
        [HideInInspector]  public BoolProperty Modal;

        public WindowViewBinder Binder;
        protected EventTrigger eventTrigger;

        protected virtual void Awake()
        {
            BindView();
            Binder = new WindowViewBinder();
            Binder.BindViewModel(gameObject.name + gameObject.GetInstanceID().ToString(), __visible, __Interact, __Modal, __Transition);
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
            if(ModalWindow != null)
            {
                Binder.ViewBinding.BindModalWindow(ModalWindow.Binder.ViewBinding);
                Binder.ViewBinding.AddChildWindow(ModalWindow.Binder.ViewBinding);
            }

            foreach (var w in ChildWindowList)
            {
                Binder.ViewBinding.AddChildWindow(w.Binder.ViewBinding);
            }

            SetVisible();
            SetInteractable();
            SetModal();
        }
        void BindView()
        {
            Visible = new BoolProperty() { Value = __visible};
            Visible.Subscribe(SetVisible);
            Interact = new BoolProperty() { Value = __Interact };
            Interact.Subscribe(SetInteractable);
            Transition = new BoolProperty() { Value = __Transition };
            Transition.Subscribe(null);
            Modal = new BoolProperty() { Value = __Modal };
            Modal.Subscribe(SetModal);
        }

        //Request ViewModel
        void SetInteractable()
        {
            if (Interact.Value == true)
            {
                Binder.ViewBinding.InteractableWindow(true);
            }
            else
            {
                Binder.ViewBinding.NonInteractableWindow(true);
            }
        }
        void SetVisible()
        {
            if (Visible.Value)
            {
                Binder.ViewBinding.ShowWindow(true);
            }
            else
            {
                Binder.ViewBinding.HideWindow(true);
            }
        }
        void SetModal()
        {
            if (Modal.Value == true)
            {
                Binder.ViewBinding.OpenModal(true);
            }
            else
            {
                Binder.ViewBinding.CloseModal(true);
            }
        }
        
        // Update View
        private void OnVisible()
        {
            Visible.Value = Binder.ViewBinding.Visible.Get();
            RenderObj.gameObject.SetActive(Binder.ViewBinding.Visible.Get());
        }
        private void OnInteractable()
        {
            Interact.Value = Binder.ViewBinding.Interactable.Get();
            eventTrigger.enabled = Binder.ViewBinding.Interactable.Get();
        }
        private void OnModal()
        {
            Modal.Value = Binder.ViewBinding.Modal.Get();
        }
        private void OnTransition()
        {
            Transition.Value = Binder.ViewBinding.Transition.Get();
        }
        private void OnClick(BaseEventData data)
        {
            PointerEventData pointerData = data as PointerEventData;
            Binder.ViewBinding.OnClick(pointerData.position);
        }
    }
}