using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 셋팅 값을 설정할 수 있습니다.
/// </summary>
/// 

//사운드 변경 시 사용 할 것
public interface SoundChange
{
    public void Change(float volume);
}

//각 셋팅에서 적용 명령을 내릴 것
public interface Setter
{
    //적용 명령
    public void Apply();
    //활성화 명령
    public void Active(bool b);
}

//게임 셋팅을 위한 버튼 연결을 위한 것
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

    //사운드 적용을 위한 전체 변수
    public static Action Action_SoundChanger = null;

    private void Awake()
    {
        //나머지 다 끄고 본인 것만 켜진다.
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
        //첫번째 옵션만 활성화
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
        }//비활성화 처리
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

    //현재 변경된 사항을 적용하기 위함
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
    //딜레이와 사용자 피드백
    IEnumerator ApplyCoroutine()
    {
        delay = true;
        img_Apply.enabled = true;

        Color fade = img_Apply.color;
        fade.a = 1.0f;
        while (fade.a > 0.0f)
        {
            fade.a -= Time.unscaledDeltaTime*5; // 0.25초
            img_Apply.color = fade;
            yield return null;
        }
        fade.a = 0;
        img_Apply.color = fade;

        img_Apply.enabled = false;
        delay = false;
    }


    //초기 로드 시 불러오기 설정
    public static void LoadInitializeSetting()
    {
        Debug.LogError("!! 시작 - 그래픽 설정을 진행합니다.");

        //퀄리티 변경
        QualitySettings.SetQualityLevel(DataSet.Instance.GraphicSetValue.Quality , true);

        //그래픽 에셋 안티 앨리어싱 설정 변경
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        urpAsset.msaaSampleCount = DataSet.Instance.GraphicSetValue.AntiAliasing;
        urpAsset.renderScale = 0.4f;

        //브이싱크 옵션 설정
        QualitySettings.vSyncCount = DataSet.Instance.GraphicSetValue.VSync;

        //프레임레이트 설정
        Application.targetFrameRate = Mathf.Clamp(DataSet.Instance.GraphicSetValue.FPS, 30, 240);

        //배경 색 설정
        RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor();

        //2  사운드 설정
        //기본음 변경
        foreach (var source in FindObjectsOfType<AudioSource>())
        {
            source.volume = DataSet.Instance.SettingValue.Volume;
        }

        //다른 것들도 변경
        Setting.Action_SoundChanger();
    }


    /*
    //Resolution
    public TMP_Dropdown dropdown_resolution;
    List<Resolution> resolutions = new List<Resolution>();//해상도를 담는다.
    public int _dropdownResolutionNum = -1; // 드롭박스 내에서 선택한 해상도 배열 번호


    //FullScrren
    public Toggle toggle_fullScreen;
    FullScreenMode fullScreenMode;

    //sound
    public Slider slider_sound;

    //Sensitivity
    public Slider slider_sensitivity;

    //Applied Image
    public Image image_applied;

    //적용 쿨타임중인지를 나타냄.
    bool applyCoolTime = false;



#if UNITY_ANDROID
    //버튼 관련
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
        Debug.Log("셋팅값 초기화.");

        //적용하기 쿨타임 적용
        applyCoolTime = false;

        //적용완료 후 색깔 지정
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


    #region 버튼
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
    //타겟 fps 지정
    public void InitButton()
    {
        Debug.LogWarning($"타겟 프레임 레이트 : {Application.targetFrameRate}");

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

    #region 설정 값 셋팅

    //해상도를 가져와 설정합니다.
    private void InitResolution()
    {
        if (!GameConfig.IsPc()) return;

            fullScreenMode = Screen.fullScreenMode;
        toggle_fullScreen.isOn = fullScreenMode == FullScreenMode.FullScreenWindow;

        

        //해상도 값들을 저장
        resolutions.Clear();
        foreach (Resolution resol in Screen.resolutions)
        {
            //Debug.Log($"{resol.width} x {resol.height} [{Mathf.CeilToInt(resol.refreshRateRatio.numerator * 0.001f)}FPS]");
            
            float ratio = resol.width / (float)resol.height;
            float ratioCompare = 1.7777777f;
            if((ratio > (ratioCompare - 0.05f)) && (ratio < (ratioCompare + 0.05f))) // 16 : 9 비율일 경우에만 실행함.
            {
                //주사율이 60hz일 경우에만 지정
                if (Mathf.CeilToInt(resol.refreshRateRatio.numerator * 0.001f) <= 63 
                    && Mathf.CeilToInt(resol.refreshRateRatio.numerator * 0.001f) >= 57)
                {
                    resolutions.Add(resol);
                }
            }
        }

        dropdown_resolution.options.Clear();//기존 옵션 내용 제거
        //드롭다운 박스에 해상도 값을 표시
        int _num = -1;
        foreach (Resolution resolution in resolutions)
        {
            _num++;

            //옵션값을 지정한다.
            TMP_Dropdown.OptionData _option = new TMP_Dropdown.OptionData();
            _option.text = string.Format($"{resolution.width} x {resolution.height}"); //[{Mathf.CeilToInt(resolution.refreshRateRatio.numerator * 0.001f)}FPS]");
            dropdown_resolution.options.Add(_option);

            //현재 옵션값을 판단하여 현재의 해상도를 표시한다.
            if (Screen.width.Equals(resolution.width) &&
                Screen.height.Equals(resolution.height) 
                //&&Screen.currentResolution.refreshRateRatio.Equals(resolution.refreshRateRatio)
                )
            {
                //아래 값들은 상황판단 변수임
                dropdown_resolution.value = _num;
                _dropdownResolutionNum = _num;

                width = resolution.width;
                height = resolution.height;
                framRate = resolution.refreshRateRatio;
            }
        }
        dropdown_resolution.RefreshShownValue();//새로고침
    }

    //사운드 값 초기화
    private void InitSound()
    {
        float _volume = DataSet.Instance.SettingValue.Volume;
        slider_sound.value = _volume;
    }

    //감도 값 초기화
    private void InitSensitivity()
    {
        float _sensitivity = DataSet.Instance.SettingValue.MouseSensitivity;
        slider_sensitivity.value = _sensitivity;
    }
    #endregion



    #region 버튼 클릭 시 적용되는 사항
    //드롭다운 박스에서 선택한 값
    public void SetResolution(int _num)
    {
        _dropdownResolutionNum = dropdown_resolution.value;
    }

    //전체화면여부 선택하기
    public void SetIsFullScreen(bool _full)
    {
        fullScreenMode = _full ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    //현재 셋팅값 적용버튼
    public void ApplyButton()
    {
        if (applyCoolTime) 
        {
            Debug.LogWarning("너무 자주 호출했습니다.");
            return;
        }

        //피시일 경우에만 허용함.
        if (GameConfig.IsPc())
        {
            Debug.LogWarning($"현재 지정된 해상도 개수 : {dropdown_resolution.options.Count}");
            //해상도 설정
            if (resolutions.Count > 0)
            {
                //Debug.LogWarning($"{resolutions[_dropdownResolutionNum].width} =? {width}");
                //Debug.LogWarning($"{resolutions[_dropdownResolutionNum].height} =? {height}");
                //Debug.LogWarning($"{Mathf.CeilToInt(resolutions[_dropdownResolutionNum].refreshRateRatio.numerator * 0.001f)} =? {framRate}");
                //Debug.LogWarning($"{Screen.fullScreenMode} =? {fullScreenMode}");

                if ((resolutions[_dropdownResolutionNum].width != width) ||
                    (resolutions[_dropdownResolutionNum].height != height) ||
                    (resolutions[_dropdownResolutionNum].refreshRateRatio.Equals(framRate)) ||
                    (Screen.fullScreenMode.Equals(fullScreenMode)))//선택한 것과 현재 해상도가 다르다면?
                {
                    //현재 선택된 번호와 실제 해상도 번호와 다른경우 변경
                    width = resolutions[_dropdownResolutionNum].width;
                    height = resolutions[_dropdownResolutionNum].height;
                    framRate = resolutions[_dropdownResolutionNum].refreshRateRatio;

                    //적용 해상도와 전체화면 여부
                    Screen.SetResolution(width, height, fullScreenMode, framRate);

                    Debug.LogWarning("해상도 변경 완료");
                }
            }
        }
            

        //sound
        DataSet.Instance.SettingValue.Volume = slider_sound.value;
        foreach (var source in FindObjectsOfType<AudioSource>()){
            source.volume = DataSet.Instance.SettingValue.Volume;
        }

        //감도
        DataSet.Instance.SettingValue.MouseSensitivity = slider_sensitivity.value;


        //언어
        if (!GameConfig.currentLanguage.Equals(LocalizationSettings.SelectedLocale.LocaleName))
        {
            foreach(var paper in  FindObjectsOfType<Paper>())
            {
                paper.ChangeLanguage();
            }

            GameConfig.currentLanguage = LocalizationSettings.SelectedLocale.LocaleName;
        }

        DataSet.Instance.Save();

        Debug.Log($"Setting값 변경이 완료 되었습니다. \n 해상도 {Screen.currentResolution.width} x {Screen.currentResolution.height} [60Hz]" +
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

    //게임 진행에 대한 정보를 설정시켜둔다.
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

        //언어
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
