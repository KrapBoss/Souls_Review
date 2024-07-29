using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
/// <summary>
/// 모바일 전용 그래픽 셋팅
/// 기본 설정 Row || 60FPS
/// </summary>
public class SettingMobile : MonoBehaviour
{
    //소리
    public Slider slider_BGM;
    public Slider slider_Effect;
    //밝기
    public Slider slider_Brightness;
    //감도
    public Slider slider_Sensitivity;

    [Header("0 : 30 | 1 :60 [FPS]")]
    public Button[] FPS_Buttons;

    public Button button_Apply;
    public Image img_Apply;

    int fps;

    private void Awake()
    {
        FPS_Buttons[0].onClick.AddListener(() => ButtonFPS(true));
        FPS_Buttons[1].onClick.AddListener(() => ButtonFPS(false));

        button_Apply.onClick.AddListener(Apply);

        slider_Brightness.onValueChanged.AddListener(SetBrightness);
    }

    private void OnEnable()
    {
        Init();

        img_Apply.enabled = false;
    }
    private void OnDisable()
    {
        ReturnSetting();
    }

    void Init()
    {
        slider_BGM.value = DataSet.Instance.SettingValue.BGMVoulme;
        slider_Effect.value = DataSet.Instance.SettingValue.Volume;
        slider_Sensitivity.value = DataSet.Instance.SettingValue.MouseSensitivity;

        slider_Brightness.value = DataSet.Instance.GraphicSetValue.Brightness;

        ButtonFPSSelect(DataSet.Instance.GraphicSetValue.FPS < 60);
    }

    void Apply()
    {
        Debug.LogError("MobileSetting 변경사항 저장");

        //언어 설정 변경에 따른 것
        if (!GameConfig.currentLanguage.Equals(LocalizationSettings.SelectedLocale.LocaleName))
        {
            foreach (var paper in FindObjectsOfType<Paper>())
            {
                paper.ChangeLanguage();
            }

            GameConfig.currentLanguage = LocalizationSettings.SelectedLocale.LocaleName;
        }

        DataSet.Instance.SettingValue.BGMVoulme = slider_BGM.value;
        DataSet.Instance.SettingValue.Volume = slider_Effect.value;
        DataSet.Instance.SettingValue.MouseSensitivity = slider_Sensitivity.value;

        //그래픽
        DataSet.Instance.GraphicSetValue.FPS = fps;
        Application.targetFrameRate = fps;

        //밝기
        DataSet.Instance.GraphicSetValue.Brightness = slider_Brightness.value;

        DataSet.Instance.Save();

        //기본음 변경
        foreach (var source in FindObjectsOfType<AudioSource>())
        {
            source.volume = DataSet.Instance.SettingValue.Volume;
        }

        //다른 것들도 변경
        Setting.Action_SoundChanger();
    }

    bool delay;
    //현재 변경된 사항을 적용하기 위함
    public void ApplyButton()
    {
        if (delay) return;

        StartCoroutine(ApplyCoroutine());
    }
    //딜레이와 사용자 피드백
    IEnumerator ApplyCoroutine()
    {
        delay = true;
        img_Apply.enabled = true;

        Color fade = img_Apply.color;
        fade.a = 1.0f;
        while (fade.a > 0.0f)
        {
            fade.a -= Time.unscaledDeltaTime * 5; // 0.25초
            img_Apply.color = fade;
            yield return null;
        }
        fade.a = 0;
        img_Apply.color = fade;

        img_Apply.enabled = false;
        delay = false;
    }

    void ButtonFPSSelect(bool is30)
    {
        if (is30)
        {
            FPS_Buttons[0].GetComponent<Image>().color = Color.white;
            FPS_Buttons[1].GetComponent<Image>().color = Color.gray;
            fps = 30;
        }
        else
        {
            FPS_Buttons[0].GetComponent<Image>().color = Color.gray;
            FPS_Buttons[1].GetComponent<Image>().color = Color.white;
            fps = 60;
        }
    }


    #region For OptionButton
    public void ButtonFPS(bool is30)
    {
        ButtonFPSSelect(is30);
    }

    //밝기 설정
    public void SetBrightness(float value)
    {
        //카메라를 장착하고 있으면 변경되어서는 안됨.
        if (CanBright()) RenderSettings.ambientLight = slider_Brightness.value * DataSet.Instance.Color_Default;
    }
    bool CanBright()
    {
        //카메라를 장착하고 있으면 변경되어서는 안됨.
        if (PlayerEvent.instance != null)
        {
            //카메라 장착중이라면?
            if (PlayerEvent.instance.cameraEquipState)
            {
                return false;
            }

            //게임 시작 후 튜토리얼이 끝나지 않았을 경우 변경 X
            if (GameManager.Instance != null)
            {
                if (!GameManager.Instance.GameStarted) return false;
            }
        }
        return true;
    }
    #endregion

    //저장되지 않은 값은 이전 값으로 변경
    void ReturnSetting()
    {
        if (CanBright())
        {
            RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor();
        }
    }

}