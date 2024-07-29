using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
/// <summary>
/// ����� ���� �׷��� ����
/// �⺻ ���� Row || 60FPS
/// </summary>
public class SettingMobile : MonoBehaviour
{
    //�Ҹ�
    public Slider slider_BGM;
    public Slider slider_Effect;
    //���
    public Slider slider_Brightness;
    //����
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
        Debug.LogError("MobileSetting ������� ����");

        //��� ���� ���濡 ���� ��
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

        //�׷���
        DataSet.Instance.GraphicSetValue.FPS = fps;
        Application.targetFrameRate = fps;

        //���
        DataSet.Instance.GraphicSetValue.Brightness = slider_Brightness.value;

        DataSet.Instance.Save();

        //�⺻�� ����
        foreach (var source in FindObjectsOfType<AudioSource>())
        {
            source.volume = DataSet.Instance.SettingValue.Volume;
        }

        //�ٸ� �͵鵵 ����
        Setting.Action_SoundChanger();
    }

    bool delay;
    //���� ����� ������ �����ϱ� ����
    public void ApplyButton()
    {
        if (delay) return;

        StartCoroutine(ApplyCoroutine());
    }
    //�����̿� ����� �ǵ��
    IEnumerator ApplyCoroutine()
    {
        delay = true;
        img_Apply.enabled = true;

        Color fade = img_Apply.color;
        fade.a = 1.0f;
        while (fade.a > 0.0f)
        {
            fade.a -= Time.unscaledDeltaTime * 5; // 0.25��
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

    //��� ����
    public void SetBrightness(float value)
    {
        //ī�޶� �����ϰ� ������ ����Ǿ�� �ȵ�.
        if (CanBright()) RenderSettings.ambientLight = slider_Brightness.value * DataSet.Instance.Color_Default;
    }
    bool CanBright()
    {
        //ī�޶� �����ϰ� ������ ����Ǿ�� �ȵ�.
        if (PlayerEvent.instance != null)
        {
            //ī�޶� �������̶��?
            if (PlayerEvent.instance.cameraEquipState)
            {
                return false;
            }

            //���� ���� �� Ʃ�丮���� ������ �ʾ��� ��� ���� X
            if (GameManager.Instance != null)
            {
                if (!GameManager.Instance.GameStarted) return false;
            }
        }
        return true;
    }
    #endregion

    //������� ���� ���� ���� ������ ����
    void ReturnSetting()
    {
        if (CanBright())
        {
            RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor();
        }
    }

}