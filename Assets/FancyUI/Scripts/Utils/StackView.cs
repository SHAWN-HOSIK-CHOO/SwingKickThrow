using System.Collections.Generic;
using UnityEngine;


namespace MVVM.Utils
{
    public class StackView : MonoBehaviour
    {
        public virtual void OnViewStackPushRequest(Stack<StackView> stack)
        {

        }

        public virtual void OnViewStackPopRequest(Stack<StackView> stack)
        {

        }
    }
}