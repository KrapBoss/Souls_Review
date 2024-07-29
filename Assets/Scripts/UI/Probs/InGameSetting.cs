using UnityEngine;

/// <summary>
/// �ΰ��� ���ð��� �����Ѵ�.
/// �⺻������ ���� �гθ� �����ϸ�, ��ư�� Ŭ���� ��� ������ �ִϸ��̼ǰ� �Բ� �ش� �г��� �����Ѵ�.
/// </summary>
public class InGameSetting : MonoBehaviour
{
    [SerializeField] GameObject go_Setting;
    [SerializeField] GameObject go_Exit;

    //���ÿ� ���� ������ �մϴ�.
    public void Init()
    {
        go_Setting.SetActive(false);
        go_Exit.SetActive(false);
    }
    
    //���� ��ư ����
    public void _ButtonSetting()
    {
        go_Setting.SetActive(!go_Setting.activeSelf);
        go_Exit.SetActive(false);
    }

    //������ ��ư ����
    public void _ButtonExit()
    {
        go_Exit.SetActive(!go_Exit.activeSelf);
        go_Setting.SetActive(false);
    }
}
