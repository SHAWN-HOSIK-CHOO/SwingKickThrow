using MVVM.Property;
using STC;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using static STC.StateChart;

namespace MVVM.View.Window
{
    [RequireComponent(typeof(RectTransform))]
    public class Window : MonoBehaviour
    {
        public GameObject RenderObj;
        [HideInInspector] protected Window __ParentWindow;
        [HideInInspector] public ModalWindow __ModalWindow;
        [HideInInspector] public List<Window> __ChildWindowList;
        [HideInInspector] public bool __visible;
        [HideInInspector] public bool __Interact;
        [HideInInspector] public bool __Modal;
        [HideInInspector] public bool __Transition;
        
        public WindowViewBinder Binder;
        protected EventTrigger eventTrigger;
        protected CanvasGroup canvasGroup;

        private bool Initialized;

        protected virtual void Awake()
        {
            eventTrigger = RenderObj.GetComponent<EventTrigger>();
            canvasGroup = RenderObj.GetComponent<CanvasGroup>();  
            EventTriggerHelper.AddTriggerEvent(eventTrigger, EventTriggerType.PointerClick, OnClick);
            Initialize();
            
        }
        protected virtual void Start()
        {
            if (Binder.WindowHierachyState.ParentState == null)
            {
                StateChart.Instance.AddChild(Binder.WindowHierachyState);
                STC.StateChart.Instance.AddTransition(new Transition(STC.StateChart.Instance.Name, Binder.WindowHierachyState.Name));
            }
        }
        
        public void Initialize()
        {
            if (Initialized == false)
            {
                Initialized = true;
                Binder = new WindowViewBinder();
                string WindowName = gameObject.name + gameObject.GetInstanceID().ToString();
                gameObject.name = WindowName;
                Binder.BindViewModel(WindowName, __visible, __Interact, __Modal, __Transition);
                Binder.BindVisibleAction(OnVisible);
                Binder.BindTransitionAction(OnTransition);
                Binder.BindModalAction(OnModal);
                Binder.BindInteractiveAction(OnInteractable);
                foreach (var w in __ChildWindowList)
                {
                    w.__ParentWindow = this;
                    w.Initialize();
                    Binder.ViewBinding.AddChildWindow(w.Binder.ViewBinding);
                }
                if (__ModalWindow != null)
                {
                    __ModalWindow.__ParentWindow = this;
                    __ModalWindow.Initialize();
                    Binder.ViewBinding.AddChildWindow(__ModalWindow.Binder.ViewBinding);
                    Binder.ViewBinding.BindModalWindow(__ModalWindow.Binder.ViewBinding);
                }
                StateChart.Instance.FlushEventList();
            }
        }


        // Update View

        public void SetInteractable(bool b)
        {
            canvasGroup.blocksRaycasts = b;
            canvasGroup.interactable = b;
            eventTrigger.enabled = b;
        }


        protected virtual void OnVisible(bool val)
        {
            var visible = Binder.ViewBinding.Visible.Get();
            canvasGroup.alpha = visible ? 1 : 0;
            SetInteractable(visible);

        }
        protected virtual void OnInteractable(bool val)
        {
            var Interactable = Binder.ViewBinding.Interactable.Get();
            SetInteractable(Interactable);
        }
        protected virtual void OnModal(bool val)
        {
            var modal = Binder.ViewBinding.Modal.Get();
            SetInteractable(!modal);
        }
        protected virtual void OnTransition(bool val)
        {
            var transition = Binder.ViewBinding.Transition.Get();
            SetInteractable(!transition);
        }
        protected virtual void OnClick(BaseEventData data)
        {
            PointerEventData pointerData = data as PointerEventData;
            Binder.ViewBinding.OnClick(pointerData.position);
        }
    }
}