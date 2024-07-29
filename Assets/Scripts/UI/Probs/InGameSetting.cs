using UnityEngine;

/// <summary>
/// 인게임 셋팅값을 조절한다.
/// 기본적으로 좌측 패널만 등장하며, 버튼을 클릭할 경우 간단한 애니메이션과 함께 해당 패널이 등장한다.
/// </summary>
public class InGameSetting : MonoBehaviour
{
    [SerializeField] GameObject go_Setting;
    [SerializeField] GameObject go_Exit;

    //셋팅에 대한 설정을 합니다.
    public void Init()
    {
        go_Setting.SetActive(false);
        go_Exit.SetActive(false);
    }
    
    //셋팅 버튼 동작
    public void _ButtonSetting()
    {
        go_Setting.SetActive(!go_Setting.activeSelf);
        go_Exit.SetActive(false);
    }

    //나가기 버튼 동작
    public void _ButtonExit()
    {
        go_Exit.SetActive(!go_Exit.activeSelf);
        go_Setting.SetActive(false);
    }
}
