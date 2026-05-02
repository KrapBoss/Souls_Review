using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI에 대한 생성을 담당하며, 새로운 UI가 스택 구조로 쌓인다.
/// 각 UI의 특성에 따라 UI를 컨트롤한다.
/// </summary>
/// 
[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
public class UIManager : MonoBehaviour
{
    static UIManager instance;
    public static UIManager Instance {
        get
        {
            if(instance == null)
            {
                GameObject go = new GameObject("UIManager");
                instance = go.AddComponent<UIManager>();
                go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            }
            return instance;
        }
    }

    /// <summary> 생성된 UI 객체를 저장 </summary>
    public List<UIElement> uiElements = new List<UIElement>();
    /// <summary> 생성된 UI의 이름을 저장 </summary>
    public List<string> uiNames = new List<string>();


    /// <summary> 원하는 UI 이름에 해당하는 UI를 띄워줍니다. </summary>
    public void Show(string uiName)
    {
        var item = Resources.Load<GameObject>(uiName);

        if(item == null)
        {
            _debug.Log($"원하는 UI가 없습니다. {uiName}");
            return;
        }

        //중복 제거
        Distinc(uiName);

        //UI 생성
        GameObject uiItem = Instantiate(item,gameObject.transform);
        //생성하려는 UI를 참조
        UIElement element = uiItem.GetComponent<UIElement>();

        try
        {
            element.Show();
            uiElements.Add(element);
            uiNames.Add(uiName);

            //활성화 확인
            ModalCheck();
        }
        catch(Exception e)
        {
            _debug.LogError(e.Message);
        }
    }

    /// <summary> 원하는 UI 이름에 해당하는 UI를 제거합니다. </summary>
    public void Back(string uiName)
    {
        int index = uiNames.IndexOf(uiName);

        if (index  < 0)
        {
            _debug.Log($"제거하려는 UI가 없습니다. {uiName}");
            return;
        }
        
        //해당 Ui를 가지고 옵니다.
        var item = uiElements[index];
        if (item != null)
        {
            //UI를 제거합니다.
            uiElements.Remove(item);
            uiNames.Remove(uiName);
            Destroy(item.gameObject);
        }

        //활성화 확인
        ModalCheck();
    }

    /// <summary> 중복을 제거합니다. </summary>
    void Distinc(string uiName)
    {
        int index = uiNames.IndexOf(uiName);

        if (index >= 0)
        {
            _debug.Log($"UI 중복 발견 => {uiName} / 중복 제거 진행 ");
            Back(uiName);
        }
    }

    /// <summary> 마지막 UI 오브젝트의 캔버스를 활성화한다. </summary>
    void ModalCheck()
    {
        if(uiElements.Count > 0)
        {
            //마지막 오브젝트는 무조건 활성화하며,
            uiElements[uiElements.Count - 1].canvasGroup.interactable = true;

            //마지막 이전의 오브젝트는 마지막 오브젝트의 모달 여부에 따라 활성화한다.
            if (uiElements.Count > 1)
                uiElements[uiElements.Count - 2].canvasGroup.interactable = !uiElements[uiElements.Count - 1].isModal;
        }
    }
}
