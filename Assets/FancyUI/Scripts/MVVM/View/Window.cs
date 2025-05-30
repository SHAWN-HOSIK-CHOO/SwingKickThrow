using MVVM.Property;
using STC;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVVM.View.Window
{
    [RequireComponent(typeof(RectTransform))]
    public class Window : MonoBehaviour
    {
        public List<Window> ChildWindowList;
        public ModalWindow ModalWindow;
        public GameObject RenderObj;

        [HideInInspector] public Window ParentWindow;
        [HideInInspector] public bool __visible;
        [HideInInspector] public bool __Interact;
        [HideInInspector] public bool __Modal;
        [HideInInspector] public bool __Transition;
        
        public WindowViewBinder Binder;
        protected EventTrigger eventTrigger;
        protected CanvasGroup canvasGroup;


        protected virtual void Awake()
        {
            Binder = new WindowViewBinder();
            Binder.BindViewModel(gameObject.name + gameObject.GetInstanceID().ToString(), __visible, __Interact, __Modal, __Transition);
            Binder.BindVisibleAction(OnVisible);
            Binder.BindTransitionAction(OnTransition);
            Binder.BindModalAction(OnModal);
            Binder.BindInteractiveAction(OnInteractable);
            eventTrigger = RenderObj.GetComponent<EventTrigger>();
            canvasGroup = RenderObj.GetComponent<CanvasGroup>();  
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
                Binder.ViewBinding.ModalVal.Value = __Modal;
            }

            foreach (var w in ChildWindowList)
            {
                Binder.ViewBinding.AddChildWindow(w.Binder.ViewBinding);
            }
        }
        
        // Update View
        protected virtual void OnVisible()
        {
            var visible = Binder.ViewBinding.Visible.Get();
            canvasGroup.alpha = visible ? 1 : 0;
        }
        protected virtual void OnInteractable()
        {
            var Interactable = Binder.ViewBinding.Interactable.Get();
            canvasGroup.blocksRaycasts = !Interactable;
        }
        protected virtual void OnModal()
        {
            var modal = Binder.ViewBinding.Modal.Get();
            canvasGroup.blocksRaycasts = modal;
        }
        protected virtual void OnTransition()
        {
            var transition = Binder.ViewBinding.Transition.Get();
        }
        protected virtual void OnClick(BaseEventData data)
        {
            PointerEventData pointerData = data as PointerEventData;
            Binder.ViewBinding.OnClick(pointerData.position);
        }
    }
}