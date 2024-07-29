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
        //우선 이름을 받아옴.
        currentLanguage = LocalizationSettings.SelectedLocale.LocaleName;

        //언어 셋팅
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
        Debug.LogWarning($"Game Config 설정 < Language{currentLanguage} | {Application.platform}>");
    }

    //pc일 경우 true
    public static bool IsPc()
    {
        if (!Platform.Equals(Application.platform)) Platform = Application.platform;
        return Platform.Equals(RuntimePlatform.WindowsPlayer);// || Platform.Equals(RuntimePlatform.WindowsEditor);
    }
}
