using CustomUI;
using UnityEngine;

/// <summary>
/// 오브젝트가 작동이 가능한 상태인지를 나타낸다.
/// 해당 오브젝트는 관련 오브젝트의 매니저에 의해 호출된다.
/// </summary>
public class ProgressObject:MonoBehaviour
{
    public bool activated;   //이미 작동을 했는가? 
    public bool canActive;  // 작동이 가능한 상태인가? // 순서가 있는 것이라면 false 가 될 것이다.

    //해당 오브젝트가 행해야 하는 동작의 내용을 가지고 있다.
    public string keyforTip="Enter The This Object Mission Content";

    ProgressObjectManager manager; // 이 오브젝트를 담당하는 매니저의 주소를 저장해둔다.

    private void Start()
    {
        GetComponent<InteractionObjects>()?.SetProgress(this);
    }

    //현재 오브젝트를 작동하는데, 가능하면 true , 불가능하면 false;
    public bool ActionObject()
    {
        //활성화 되지 않은 오브젝트일 경우
        if (!canActive)
        {
            Debug.Log($"현재 오브젝트의 상호작용이 불가능.");
            //다음으로 실행할 동작을 알려줍니다.
            manager.ShowNextAction();
            return false;
        }
        else
        {
            Debug.Log($"현재 오브젝트의 상호작용 가능.");
            activated = true;
            manager.CompleteAction(this);
            return true;
        }
    }

    public void ShowNotice() // 현재 필요한 안내문을 보여준다.
    {
        UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", keyforTip), false);
    }
    public void SetManager(in ProgressObjectManager _manager) { manager = _manager; }
}
