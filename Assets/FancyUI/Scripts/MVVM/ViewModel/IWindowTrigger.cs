using MVVM.Property;
using UnityEngine;
namespace MVVM.ViewModel.Window.Trigger
{
    public interface IWindowTrigger
    {
        BoolProperty VisibleTrigger { get;}
        BoolProperty InteractableTrigger { get;  }
        BoolProperty ModalTrigger { get;  }
        BoolProperty TransitionTrigger { get;  }
    }
}