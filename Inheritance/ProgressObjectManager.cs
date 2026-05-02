using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 진행에 있어 오브젝트들의 관리자로 이 오브젝트를 상속받으면 관리자가 된다.
/// 관리자는 관련 오브젝트의 동작이 모두 완료 되어야 실행이 가능하다.
/// </summary>
public class ProgressObjectManager : MonoBehaviour
{
    public ProgressObject[] progressObjects;
    public bool isSequence = false; // 순서가 존재하는 오브젝트인가?

    //모든 오브젝트의 동작이 완료되면 
    public bool completed;

    //모든 미션이 완료되고 난 후의 할 일을 적는다.
    public string str_key;


    private void Start()
    {
        Init();
    }

    //private :

    //초기 관리 대상들을 셋팅하는데, 필수적으로 호출해야 된다.
    protected void Init()
    {
        foreach (var progressObject in progressObjects)
        {
            progressObject.SetManager(this);
            //순서가 상관 있으면 모두 실행 불가 상태로 변경한다.
            progressObject.canActive = isSequence ? false : true;
        }
        //만약 순서대로 진행해야 되는 오브젝트라면 0번째 오브젝트는 활성화 시킨다.
        if (isSequence) progressObjects[0].canActive = true;
    }

    //모든 관련 오브젝트의 동작들이 완료된 상태를 나타낸다.
    bool AllActionComplete()
    {
        //모든 동작들이 완료 되었는지 판단합니다.
        for (int i = 0; i < progressObjects.Length; i++)
        {
            if (!progressObjects[i].activated) return false;
        }
        return true;
    }



//public :

    //다음 행동할 것을 알려준다.
    public void ShowNextAction()
    {
        for (int i = 0; i < progressObjects.Length; i++)
        {
            if (!progressObjects[i].activated)
            {
                progressObjects[i].ShowNotice();
                return;
            }
        }

        //지정된 모든 동작을 완료하면 최종 목표를 보여준다.
        UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", str_key), true);
    }

    //오브젝트들이 액션을 완료했다고 보고를 받습니다.
    public void CompleteAction(in ProgressObject _progressObject)
    {
        //순서가 존재하는 오브젝트의 경우 다음 오브젝트를 활성화 시켜줍니다.
        if (isSequence)
        {
            for (int i = 0; i < progressObjects.Length; i++)
            {
                if (_progressObject == progressObjects[i])
                {
                    if (i < progressObjects.Length - 1)
                    {
                        //다음 오브젝트가 동작 가능한 상태로 변경한다.
                        progressObjects[i + 1].canActive = true;
                    }
                }
            }
        }

        //작업이 완료 되었으면 다음 동작을 안내해줍니다.
        ShowNextAction();
    }

    // 현재 오브젝트가 실행될 때 실행이 가능하면 true, 안되면 false
    public bool ActionObject()
    {
        if (completed)
        {
            return true;
        }

        if (AllActionComplete())
        {
            Debug.Log($"모든 이벤트 조건을 충족하여 완료합니다.");
            completed = true;
            return true;
        }
        else
        {
            Debug.Log($"아직 이전 단계의 이벤트가 충족되지 않아 작동되지 않습니다.");
            ShowNextAction();
            return false;
        }
    }
}
