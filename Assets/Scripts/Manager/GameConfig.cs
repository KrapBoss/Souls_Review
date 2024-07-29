using UnityEngine;
using UnityEngine.Localization.Settings;

public class GameConfig : MonoBehaviour
{
#if UNITY_EDITOR
    public static RuntimePlatform Platform = RuntimePlatform.Android;
#else
    public static RuntimePlatform Platform = RuntimePlatform.WindowsPlayer;
#endif
    public static string currentLanguage;
    public static bool setting = false;

    // Start is called before the first frame update
    void Start()
    {
        if(setting) { Destroy(this); return; }
        //�켱 �̸��� �޾ƿ�.
        currentLanguage = LocalizationSettings.SelectedLocale.LocaleName;

        //��� ����
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Korean: //2
                LocalLanguageSetting.Instance.ChangeLocale(2);
                break;
            case SystemLanguage.Japanese: //1
                LocalLanguageSetting.Instance.ChangeLocale(1);
                break;
            default:// 0
                LocalLanguageSetting.Instance.ChangeLocale(0);
                break;
        }

        setting = true;
        Debug.LogWarning($"Game Config ���� < Language{currentLanguage} | {Application.platform}>");
    }

    //pc�� ��� true
    public static bool IsPc()
    {
        if (!Platform.Equals(Application.platform)) Platform = Application.platform;
        return Platform.Equals(RuntimePlatform.WindowsPlayer);// || Platform.Equals(RuntimePlatform.WindowsEditor);
    }
}
