using CustomUI;

/// <summary>
/// ���� ��ȣ�ۿ��� �� ��� ���� �ؽ�Ʈ�� ���ϴ�.
/// </summary>
public class DoorShowLine : Door
{
    public string Key;
    string txt;

    protected override bool LockCheck()
    {
        bool _lock= base.LockCheck();

        //����ִ� ��� ���尡 ����˴ϴ�.
        if(_lock)
        {
            txt = LocalLanguageSetting.Instance.GetLocalText("Story", Key);
            UI.staticUI.EnterLine(txt, 5.0f);
            UI.staticUI.ShowLine();
        }

        return _lock;
    }
}
