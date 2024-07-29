using CustomUI;
using UnityEngine;

/// <summary>
/// ������Ʈ�� �۵��� ������ ���������� ��Ÿ����.
/// �ش� ������Ʈ�� ���� ������Ʈ�� �Ŵ����� ���� ȣ��ȴ�.
/// </summary>
public class ProgressObject:MonoBehaviour
{
    public bool activated;   //�̹� �۵��� �ߴ°�? 
    public bool canActive;  // �۵��� ������ �����ΰ�? // ������ �ִ� ���̶�� false �� �� ���̴�.

    //�ش� ������Ʈ�� ���ؾ� �ϴ� ������ ������ ������ �ִ�.
    public string keyforTip="Enter The This Object Mission Content";

    ProgressObjectManager manager; // �� ������Ʈ�� ����ϴ� �Ŵ����� �ּҸ� �����صд�.

    private void Start()
    {
        GetComponent<InteractionObjects>()?.SetProgress(this);
    }

    //���� ������Ʈ�� �۵��ϴµ�, �����ϸ� true , �Ұ����ϸ� false;
    public bool ActionObject()
    {
        //Ȱ��ȭ ���� ���� ������Ʈ�� ���
        if (!canActive)
        {
            Debug.Log($"���� ������Ʈ�� ��ȣ�ۿ��� �Ұ���.");
            //�������� ������ ������ �˷��ݴϴ�.
            manager.ShowNextAction();
            return false;
        }
        else
        {
            Debug.Log($"���� ������Ʈ�� ��ȣ�ۿ� ����.");
            activated = true;
            manager.CompleteAction(this);
            return true;
        }
    }

    public void ShowNotice() // ���� �ʿ��� �ȳ����� �����ش�.
    {
        UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", keyforTip), false);
    }
    public void SetManager(in ProgressObjectManager _manager) { manager = _manager; }
}
