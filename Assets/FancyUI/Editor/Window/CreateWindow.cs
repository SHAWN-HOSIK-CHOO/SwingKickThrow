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
        GameObject pannel = new GameObject("Pannel", typeof(RectTransform));
        pannel.AddComponent<Image>();
        pannel.gameObject.transform.SetParent(renderObj.transform, false);

        Undo.RegisterCreatedObjectUndo(renderObj, "Create RenderObj");
        renderObj.transform.SetParent(windowGO.transform, false);
        Window w = windowGO.GetComponent<Window>();
        w.__visible = true;
        w.__Interact = true;
        w.RenderObj = renderObj;
        w.__ModalWindow = CreateModalWindowObj(windowGO).GetComponent<ModalWindow>();
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
        GameObject pannel = new GameObject("Pannel", typeof(RectTransform));
        pannel.AddComponent<Image>();
        pannel.gameObject.transform.SetParent(renderObj.transform, false);

        GameObject button = new GameObject("CloseButton", typeof(RectTransform));
        button.AddComponent<Image>();   
        button.AddComponent<Button>();
        button.transform.SetParent(renderObj.transform, false);
        Undo.RegisterCreatedObjectUndo(renderObj, "Create RenderObj");
        renderObj.transform.SetParent(windowGO.transform, false);
        renderObj.AddComponent<CanvasGroup>();
        renderObj.AddComponent<EventTrigger>();
        ModalWindow w = windowGO.GetComponent<ModalWindow>();
        w.__visible = true;
        w.__Interact = true;
        w.RenderObj = renderObj;
        w.closeButton = button.GetComponent<Button>();
        return windowGO;
    }
}
