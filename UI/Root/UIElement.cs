using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  각 UI 객체에 대한 Resources를 통한 생성이 필요한 경우 및 조건에 따른 추가 제재가 있을 경우 상속받아 사용
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UIElement : MonoBehaviour
{
    /// <summary> 하위 UI의 동작을 막을 것이면 true </summary>
    public bool isModal;

    /// <summary> 내 그룹 </summary>
    public CanvasGroup canvasGroup;

    /// <summary> 보여질 때 사용하는 것 </summary>
    public virtual void Show()
    {
        _debug.Log($"UI 생성 {name}");

        canvasGroup = GetComponent<CanvasGroup>();
    }


    /// <summary> 사라질 때 사용하는 것 </summary>
    public virtual void Back()
    {
        _debug.Log($"UI 제거 {name}");

        //나를 제거해주세요.
        UIManager.Instance.Back(this.name);
    }
}
