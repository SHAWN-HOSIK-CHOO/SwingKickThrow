using STC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static STC.StateChart;

namespace MVVM.View.Window
{
    [RequireComponent(typeof(RectTransform))]
    public class Window : MonoBehaviour
    {
        public RenderObj RenderObj;
        [HideInInspector] protected Window __ParentWindow;
        [HideInInspector] public ModalWindow __ModalWindow;
        [HideInInspector] public List<Window> __ChildWindowList;
        [HideInInspector] public bool __visible;
        [HideInInspector] public bool __Interact;
        [HideInInspector] public bool __Modal;
        [HideInInspector] public bool __Transition;
        
        public  WindowViewBinder Binder;
        protected EventTrigger eventTrigger;
        protected CanvasGroup canvasGroup;
        private bool Initialized;

        public event Action<bool> OnVisibleCB;
        public event Action<bool> OnInteractableCB;
        public event Action<bool> OnModalCB;
        public event Action<bool> OnTransitionCB;

        protected virtual void Awake()
        {
            eventTrigger = RenderObj.GetComponent<EventTrigger>();
            canvasGroup = RenderObj.GetComponent<CanvasGroup>();  
            Initialize();
        }
        protected virtual void Start()
        {
            if (Binder.WindowHierachyState.ParentState == null)
            {
                StateChart.Instance.AddChild(Binder.WindowHierachyState);
                STC.StateChart.Instance.AddTransition(new STC.Transition(STC.StateChart.Instance.Name, Binder.WindowHierachyState.Name));
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
        void SetInteractable(bool b)
        {
            if (this == null) return;
            canvasGroup.interactable = b;
            eventTrigger.enabled = b;
        }
        protected void OnVisible(bool val)
        {
            var visible = Binder.ViewBinding.Visible.Get();
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.blocksRaycasts = visible;
            SetInteractable(visible);
            OnVisibleCB?.Invoke(val);

        }
        protected void OnInteractable(bool val)
        {
            var Interactable = Binder.ViewBinding.Interactable.Get();
            SetInteractable(Interactable);
            OnInteractableCB?.Invoke(val);
        }
        protected void OnModal(bool val)
        {
            var modal = Binder.ViewBinding.Modal.Get();
            SetInteractable(!modal);
            OnModalCB?.Invoke(val);
        }
        protected void OnTransition(bool val)
        {
            var transition = Binder.ViewBinding.Transition.Get();
            SetInteractable(!transition);
            OnTransitionCB?.Invoke(val);
        }    
        public void TransitionStart()
        {
            Binder.ViewBinding.TransitionStart();
        }
        public void ResetTransition()
        {
            RenderObj.ResetTransition();
        }
        public void TransitionEnd()
        {
            Binder.ViewBinding.TransitionEnd();
        }
        public virtual void CloseModal()
        {
            Binder.ViewBinding.CloseModal();
        }
        public void OpenModal()
        {
            Binder.ViewBinding.OpenModal();
        }
        public void ShowWindow()
        {
            Binder.ViewBinding.ShowWindow();
        }
        public void Interactable()
        {
            Binder.ViewBinding.InteractableWindow();
        }
        public void NonInteractable()
        {
            Binder.ViewBinding.NonInteractableWindow();
        }
        public void HideWindow()
        {
            Binder.ViewBinding.HideWindow();
        }
    }
}