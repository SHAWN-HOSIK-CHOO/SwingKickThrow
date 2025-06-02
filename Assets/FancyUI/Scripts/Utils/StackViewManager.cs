using System.Collections.Generic;

namespace MVVM.Utils
{
    public class StackViewManager
    {
        Stack<MVVM.Utils.StackView> ViewStack;
        private static StackViewManager _instance;
        public static StackViewManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StackViewManager();
                }
                return _instance;
            }
        }

        public int StackCount => ViewStack.Count;


        StackViewManager()
        {
            ViewStack = new Stack<StackView>();
        }

        public void Pop()
        {
            if (ViewStack.Count > 0)
            {
                var topView = ViewStack.Peek();
                topView.OnViewStackPopRequest(ViewStack);
            }
        }
        public void Push(MVVM.Utils.StackView view)
        {
            view.OnViewStackPushRequest(ViewStack);
        }

        public void Clear()
        {
            ViewStack.Clear();
        }

        public bool IsSame (MVVM.Utils.StackView view)
        {
            if (ViewStack.Count > 0)
            {
                var topView = ViewStack.Peek();
                return Equals(topView,view);
            }
            return false;
        }

        public bool IsEmpty()
        {
            return ViewStack.Count == 0;
        }
    }
}