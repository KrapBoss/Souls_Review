using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ���� ���� ������ �� �ֽ��ϴ�.
/// </summary>
/// 

//���� ���� �� ��� �� ��
public interface SoundChange
{
    public void Change(float volume);
}

//�� ���ÿ��� ���� ����� ���� ��
public interface Setter
{
    //���� ���
    public void Apply();
    //Ȱ��ȭ ���
    public void Active(bool b);
}

//���� ������ ���� ��ư ������ ���� ��
[System.Serializable]
public struct SettingButtonConnect
{
    public Button button;
    public GameObject go_Active;
    public Setter setter;
}

public class Setting : MonoBehaviour
{
    [SerializeField]
    SettingButtonConnect[] buttonConnect;

    public Color c_ActiveButton;
    public Color c_DeactiveButton;

    public Button btn_Apply;
    public Image img_Apply;
    bool delay;

    //���� ������ ���� ��ü ����
    public static Action Action_SoundChanger = null;

    private void Awake()
    {
        //������ �� ���� ���� �͸� ������.
        for (int i = 0; i<buttonConnect.Length; i++)
        {
            buttonConnect[i].setter = buttonConnect[i].go_Active.GetComponent<Setter>();

            int index = i;
            buttonConnect[i].button.onClick.AddListener(() => ActiveOptionPanel(index));
        }

        btn_Apply.onClick.AddListener(ApplyButton);
    }

    private void OnEnable()
    {
        //ù��° �ɼǸ� Ȱ��ȭ
        if(buttonConnect.Length > 0)
        {
            foreach (var button in buttonConnect)
            {
                button.button.GetComponent<Image>().color = c_DeactiveButton;

                if (button.setter != null)
                    button.setter.Active(false);
            }

            buttonConnect[0].setter.Active(true);
            buttonConnect[0].button.GetComponent<Image>().color = c_ActiveButton;
        }

        img_Apply.enabled = false;
        delay = false;
    }

    private void OnDisable()
    {
        foreach (var button in buttonConnect)
        {
            button.setter.Active(false);
        }//��Ȱ��ȭ ó��
        StopAllCoroutines();
    }

    public void ActiveOptionPanel(int index)
    {
        foreach(var button in buttonConnect)
        {
            if (button.setter != null)
                button.setter.Active(false);
            button.button.GetComponent<Image>().color = c_DeactiveButton;
        }
        buttonConnect[index].setter.Active(true);

        buttonConnect[index].button.GetComponent<Image>().color = c_ActiveButton;
    }

    //���� ����� ������ �����ϱ� ����
    public void ApplyButton()
    {
        if (delay) return;

        foreach (var button in buttonConnect)
        {
            if (button.setter != null && button.go_Active.activeSelf)
                button.setter.Apply();
        }

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
            fade.a -= Time.unscaledDeltaTime*5; // 0.25��
            img_Apply.color = fade;
            yield return null;
        }
        fade.a = 0;
        img_Apply.color = fade;

        img_Apply.enabled = false;
        delay = false;
    }


    //�ʱ� �ε� �� �ҷ����� ����
    public static void LoadInitializeSetting()
    {
        Debug.LogError("!! ���� - �׷��� ������ �����մϴ�.");

        //����Ƽ ����
        QualitySettings.SetQualityLevel(DataSet.Instance.GraphicSetValue.Quality , true);

        //�׷��� ���� ��Ƽ �ٸ���� ���� ����
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        urpAsset.msaaSampleCount = DataSet.Instance.GraphicSetValue.AntiAliasing;
        urpAsset.renderScale = 0.4f;

        //���̽�ũ �ɼ� ����
        QualitySettings.vSyncCount = DataSet.Instance.GraphicSetValue.VSync;

        //�����ӷ���Ʈ ����
        Application.targetFrameRate = Mathf.Clamp(DataSet.Instance.GraphicSetValue.FPS, 30, 240);

        //��� �� ����
        RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor();

        //2  ���� ����
        //�⺻�� ����
        foreach (var source in FindObjectsOfType<AudioSource>())
        {
            source.volume = DataSet.Instance.SettingValue.Volume;
        }

        //�ٸ� �͵鵵 ����
        Setting.Action_SoundChanger();
    }


    /*
    //Resolution
    public TMP_Dropdown dropdown_resolution;
    List<Resolution> resolutions = new List<Resolution>();//�ػ󵵸� ��´�.
    public int _dropdownResolutionNum = -1; // ��ӹڽ� ������ ������ �ػ� �迭 ��ȣ


    //FullScrren
    public Toggle toggle_fullScreen;
    FullScreenMode fullScreenMode;

    //sound
    public Slider slider_sound;

    //Sensitivity
    public Slider slider_sensitivity;

    //Applied Image
    public Image image_applied;

    //���� ��Ÿ���������� ��Ÿ��.
    bool applyCoolTime = false;



#if UNITY_ANDROID
    //��ư ����
    public Sprite img_Default;
    public Sprite img_Selected;
    public Button btn_30;
    public Button btn_60;
#endif

    int width, height;
    RefreshRate framRate;

//    private void Start()
//    {
//        if (GameConfig.IsPc())
//        {
//            dropdown_resolution.onValueChanged.AddListener(SetResolution);
//            toggle_fullScreen.onValueChanged.AddListener(SetIsFullScreen);
//        }
//#if UNITY_ANDROID
//        else
//        {
            
//            if(btn_30 != null)
//            {
//                btn_30.onClick.AddListener(ButtonFrame30);
//                btn_60.onClick.AddListener(ButtonFrame60);
//            }
//        }
//#endif
//    }

    //private void OnEnable()
    //{
    //    Init();
    //}

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Init()
    {
        Debug.Log("���ð� �ʱ�ȭ.");

        //�����ϱ� ��Ÿ�� ����
        applyCoolTime = false;

        //����Ϸ� �� ���� ����
        Color color = image_applied.color;
        color.a = 0.0f;
        image_applied.color = color;


        InitResolution();
        InitSound();
        InitSensitivity();

#if UNITY_ANDROID
        InitButton();
#endif
    }


    #region ��ư
#if UNITY_ANDROID
    public void ButtonFrame30()
    {
        Application.targetFrameRate = 30;
        btn_30.GetComponent<Image>().sprite = img_Selected;
        btn_60.GetComponent<Image>().sprite = img_Default;
    }
    public void ButtonFrame60()
    {
        Application.targetFrameRate = 60;
        btn_60.GetComponent<Image>().sprite = img_Selected;
        btn_30.GetComponent<Image>().sprite = img_Default;
    }
    //Ÿ�� fps ����
    public void InitButton()
    {
        Debug.LogWarning($"Ÿ�� ������ ����Ʈ : {Application.targetFrameRate}");

        if(Application.targetFrameRate == 30 || Application.targetFrameRate == -1)
        {
            btn_30.GetComponent<Image>().sprite = img_Selected;
            btn_60.GetComponent<Image>().sprite = img_Default;
        }
        else
        {
            btn_60.GetComponent<Image>().sprite = img_Selected;
            btn_30.GetComponent<Image>().sprite = img_Default;
        }
    }
#endif
    #endregion

    #region ���� �� ����

    //�ػ󵵸� ������ �����մϴ�.
    private void InitResolution()
    {
        if (!GameConfig.IsPc()) return;

            fullScreenMode = Screen.fullScreenMode;
        toggle_fullScreen.isOn = fullScreenMode == FullScreenMode.FullScreenWindow;

        

        //�ػ� ������ ����
        resolutions.Clear();
        foreach (Resolution resol in Screen.resolutions)
        {
            //Debug.Log($"{resol.width} x {resol.height} [{Mathf.CeilToInt(resol.refreshRateRatio.numerator * 0.001f)}FPS]");
            
            float ratio = resol.width / (float)resol.height;
            float ratioCompare = 1.7777777f;
            if((ratio > (ratioCompare - 0.05f)) && (ratio < (ratioCompare + 0.05f))) // 16 : 9 ������ ��쿡�� ������.
            {
                //�ֻ����� 60hz�� ��쿡�� ����
                if (Mathf.CeilToInt(resol.refreshRateRatio.numerator * 0.001f) <= 63 
                    && Mathf.CeilToInt(resol.refreshRateRatio.numerator * 0.001f) >= 57)
                {
                    resolutions.Add(resol);
                }
            }
        }

        dropdown_resolution.options.Clear();//���� �ɼ� ���� ����
        //��Ӵٿ� �ڽ��� �ػ� ���� ǥ��
        int _num = -1;
        foreach (Resolution resolution in resolutions)
        {
            _num++;

            //�ɼǰ��� �����Ѵ�.
            TMP_Dropdown.OptionData _option = new TMP_Dropdown.OptionData();
            _option.text = string.Format($"{resolution.width} x {resolution.height}"); //[{Mathf.CeilToInt(resolution.refreshRateRatio.numerator * 0.001f)}FPS]");
            dropdown_resolution.options.Add(_option);

            //���� �ɼǰ��� �Ǵ��Ͽ� ������ �ػ󵵸� ǥ���Ѵ�.
            if (Screen.width.Equals(resolution.width) &&
                Screen.height.Equals(resolution.height) 
                //&&Screen.currentResolution.refreshRateRatio.Equals(resolution.refreshRateRatio)
                )
            {
                //�Ʒ� ������ ��Ȳ�Ǵ� ������
                dropdown_resolution.value = _num;
                _dropdownResolutionNum = _num;

                width = resolution.width;
                height = resolution.height;
                framRate = resolution.refreshRateRatio;
            }
        }
        dropdown_resolution.RefreshShownValue();//���ΰ�ħ
    }

    //���� �� �ʱ�ȭ
    private void InitSound()
    {
        float _volume = DataSet.Instance.SettingValue.Volume;
        slider_sound.value = _volume;
    }

    //���� �� �ʱ�ȭ
    private void InitSensitivity()
    {
        float _sensitivity = DataSet.Instance.SettingValue.MouseSensitivity;
        slider_sensitivity.value = _sensitivity;
    }
    #endregion



    #region ��ư Ŭ�� �� ����Ǵ� ����
    //��Ӵٿ� �ڽ����� ������ ��
    public void SetResolution(int _num)
    {
        _dropdownResolutionNum = dropdown_resolution.value;
    }

    //��üȭ�鿩�� �����ϱ�
    public void SetIsFullScreen(bool _full)
    {
        fullScreenMode = _full ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    //���� ���ð� �����ư
    public void ApplyButton()
    {
        if (applyCoolTime) 
        {
            Debug.LogWarning("�ʹ� ���� ȣ���߽��ϴ�.");
            return;
        }

        //�ǽ��� ��쿡�� �����.
        if (GameConfig.IsPc())
        {
            Debug.LogWarning($"���� ������ �ػ� ���� : {dropdown_resolution.options.Count}");
            //�ػ� ����
            if (resolutions.Count > 0)
            {
                //Debug.LogWarning($"{resolutions[_dropdownResolutionNum].width} =? {width}");
                //Debug.LogWarning($"{resolutions[_dropdownResolutionNum].height} =? {height}");
                //Debug.LogWarning($"{Mathf.CeilToInt(resolutions[_dropdownResolutionNum].refreshRateRatio.numerator * 0.001f)} =? {framRate}");
                //Debug.LogWarning($"{Screen.fullScreenMode} =? {fullScreenMode}");

                if ((resolutions[_dropdownResolutionNum].width != width) ||
                    (resolutions[_dropdownResolutionNum].height != height) ||
                    (resolutions[_dropdownResolutionNum].refreshRateRatio.Equals(framRate)) ||
                    (Screen.fullScreenMode.Equals(fullScreenMode)))//������ �Ͱ� ���� �ػ󵵰� �ٸ��ٸ�?
                {
                    //���� ���õ� ��ȣ�� ���� �ػ� ��ȣ�� �ٸ���� ����
                    width = resolutions[_dropdownResolutionNum].width;
                    height = resolutions[_dropdownResolutionNum].height;
                    framRate = resolutions[_dropdownResolutionNum].refreshRateRatio;

                    //���� �ػ󵵿� ��üȭ�� ����
                    Screen.SetResolution(width, height, fullScreenMode, framRate);

                    Debug.LogWarning("�ػ� ���� �Ϸ�");
                }
            }
        }
            

        //sound
        DataSet.Instance.SettingValue.Volume = slider_sound.value;
        foreach (var source in FindObjectsOfType<AudioSource>()){
            source.volume = DataSet.Instance.SettingValue.Volume;
        }

        //����
        DataSet.Instance.SettingValue.MouseSensitivity = slider_sensitivity.value;


        //���
        if (!GameConfig.currentLanguage.Equals(LocalizationSettings.SelectedLocale.LocaleName))
        {
            foreach(var paper in  FindObjectsOfType<Paper>())
            {
                paper.ChangeLanguage();
            }

            GameConfig.currentLanguage = LocalizationSettings.SelectedLocale.LocaleName;
        }

        DataSet.Instance.Save();

        Debug.Log($"Setting�� ������ �Ϸ� �Ǿ����ϴ�. \n �ػ� {Screen.currentResolution.width} x {Screen.currentResolution.height} [60Hz]" +
            $"\nvolume = {DataSet.Instance.SettingValue.Volume} / MouseSensitivity = {DataSet.Instance.SettingValue.MouseSensitivity}");



        applyCoolTime = true;

        StartCoroutine(AppliedCoroutine());
        StartCoroutine(ApplyCoolTimeCroutine());
    }

    IEnumerator AppliedCoroutine()
    {
        Color fade = image_applied.color;
        fade.a = 1.0f;
        while (fade.a > 0.05f)
        {
            fade.a -= Time.unscaledDeltaTime;
            image_applied.color = fade;
            yield return null;
        }
        fade.a = 0;
        image_applied.color = fade;
    }

    IEnumerator ApplyCoolTimeCroutine()
    {
        float t = 0;
        while (applyCoolTime)
        {
            t += Time.unscaledDeltaTime;
            if (t > 0.5f)
            {
                applyCoolTime = false;
            }

            yield return null;
        }
    }
    #endregion

    //���� ���࿡ ���� ������ �������ѵд�.
    public static void ApplyGameSetting()
    {
        if (!GameConfig.IsPc())
        {

            //AutoRotate
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToPortrait = false;
        }

        //sound
        foreach (var source in FindObjectsOfType<AudioSource>())
        {
            source.volume = DataSet.Instance.SettingValue.Volume;
        }
        
        if(GameConfig.currentLanguage == null)
        {
            GameConfig.currentLanguage = LocalizationSettings.SelectedLocale.LocaleName;
        }

        //���
        if (!GameConfig.currentLanguage.Equals(LocalizationSettings.SelectedLocale.LocaleName))
        {
            foreach (var paper in FindObjectsOfType<Paper>())
            {
                paper.ChangeLanguage();
            }

            GameConfig.currentLanguage = LocalizationSettings.SelectedLocale.LocaleName;
        }
    }*/
}
