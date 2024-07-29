using CustomUI;

/// <summary>
/// 문과 상호작용을 할 경우 다음 텍스트를 띄웁니다.
/// </summary>
public class DoorShowLine : Door
{
    public string Key;
    string txt;

    protected override bool LockCheck()
    {
        bool _lock= base.LockCheck();

        //잠겨있는 경우 사운드가 재생됩니다.
        if(_lock)
        {
            txt = LocalLanguageSetting.Instance.GetLocalText("Story", Key);
            UI.staticUI.EnterLine(txt, 5.0f);
            UI.staticUI.ShowLine();
        }

        return _lock;
    }
}
