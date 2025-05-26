using UnityEngine;

namespace MVVM.ViewModel.Window.Event
{
    public struct SetModalEvent : STC.Event.IStateEvent
    {
        public bool active;
    }
}