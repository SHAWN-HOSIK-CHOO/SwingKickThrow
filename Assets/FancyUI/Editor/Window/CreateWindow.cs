using UnityEngine;
using UnityEditor;
using MVVM.View.Window;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;
public class CreateWindowView 
{
    [MenuItem("GameObject/UI/MVVM/Window", false, 10)]
    static void CreateWindow(MenuCommand menuCommand)
    {
        // 현재 선택된 GameObject 기준으로 생성
        GameObject parent = Selection.activeGameObject;

        // Window GameObject 생성
        GameObject windowGO = new GameObject("Window", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(windowGO, "Create Window");
        // 컴포넌트 추가
        windowGO.AddComponent<Window>();
        Window w = windowGO.GetComponent<Window>();

        // RectTransform 초기값 설정
        RectTransform rect = windowGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 200);

        // 부모 설정
        if (parent != null)
        {
            windowGO.transform.SetParent(parent.transform, false);
        }

        // 자식인 RenderObj 생성
        GameObject renderObj = new GameObject("RenderObj", typeof(RectTransform));
        renderObj.AddComponent<CanvasGroup>();
        renderObj.AddComponent<EventTrigger>();
        renderObj.AddComponent<RenderTransition>().Owner = w;

        Undo.RegisterCreatedObjectUndo(renderObj, "Create RenderObj");
        renderObj.transform.SetParent(windowGO.transform, false);
        StretchToParent(renderObj.GetComponent<RectTransform>());
        w.__visible = true;
        w.__Interact = true;
        w.RenderObj = renderObj;
        w.__ModalWindow = CreateModalWindowObj(windowGO).GetComponent<ModalWindow>();
        StretchToParent(w.__ModalWindow.GetComponent<RectTransform>());

        // 선택 항목을 새로 생성한 Window로 지정
        Selection.activeGameObject = windowGO;
    }

    [MenuItem("GameObject/UI/MVVM/ModalWindow", false, 10)]
    static void CreateModalWindow(MenuCommand menuCommand)
    {
        // 현재 선택된 GameObject 기준으로 생성
        GameObject parent = Selection.activeGameObject;
        Selection.activeGameObject = CreateModalWindowObj(parent);
    }

    static GameObject CreateModalWindowObj(GameObject parent)
    {
        // Window GameObject 생성
        GameObject windowGO = new GameObject("Window", typeof(RectTransform));
        windowGO.name = "ModalWindow";
        Undo.RegisterCreatedObjectUndo(windowGO, "Create Window");

        // 컴포넌트 추가
        windowGO.AddComponent<ModalWindow>();
        ModalWindow w = windowGO.GetComponent<ModalWindow>();
        // RectTransform 초기값 설정
        RectTransform rect = windowGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 200);

        // 부모 설정
        if (parent != null)
        {
            windowGO.transform.SetParent(parent.transform, false);
        }

        // 자식인 RenderObj 생성
        GameObject renderObj = new GameObject("RenderObj", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(renderObj, "Create RenderObj");
        renderObj.transform.SetParent(windowGO.transform, false);
        StretchToParent(renderObj.GetComponent<RectTransform>());
        renderObj.AddComponent<CanvasGroup>();
        renderObj.AddComponent<EventTrigger>();
        renderObj.AddComponent<RenderTransition>().Owner = w;
            
            
        GameObject button = new GameObject("CloseButton", typeof(RectTransform));
        button.AddComponent<Image>();
        button.AddComponent<Button>();
        button.transform.SetParent(renderObj.transform, false);

        w.__visible = true;
        w.__Interact = true;
        w.RenderObj = renderObj;
        w.closeButton = button.GetComponent<Button>();
        return windowGO;
    }

    public static void StretchToParent(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
