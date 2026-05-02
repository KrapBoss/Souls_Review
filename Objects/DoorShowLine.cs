using CustomUI;

/// <summary>
/// 문과 상호작용을 할 경우 다음 텍스트를 띄웁니다.
/// </summary>
public class DoorShowLine : Door
{
    public string Table = "Story";
    public string Key;
    string txt;

    bool term = false;
    protected override bool LockCheck()
    {
        bool _lock= base.LockCheck();

        if (!term)
        {
            term = true;
            //잠겨있는 경우 사운드가 재생됩니다.
            if (_lock && !string.IsNullOrEmpty(Key))
            {
                if (Table.Contains("Voice"))
                {
                    VoiceLineManager.Instance.EnterLine(new() { clip = Key, delay = 1, line = Key, reverb = false });
                    VoiceLineManager.Instance.ShowLine();
                }
                else
                if (!Key.Contains("//")) // 사용하지 않는 경우 
                {
                    txt = LocalLanguageSetting.Instance.GetLocalText(Table, Key);
                    UI.staticUI.EnterLine(txt, 5.0f);
                    UI.staticUI.ShowLine();
                }
            }

            Invoke("Delay", 2.0f);
        }

        return _lock;
    }

    void Delay()
    {
        term = false;
    }
}
