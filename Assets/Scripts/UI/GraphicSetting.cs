using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GraphicSetting : MonoBehaviour, Setter
{
    //Resolution
    public TMP_Dropdown dropdown_resolution;
    List<Resolution> resolutions = new List<Resolution>();//해상도를 담는다.
    public int _dropdownResolutionNum = -1; // 드롭박스 내에서 선택한 해상도 배열 번호

    //현재 지정되어 있는 해상도 정보
    int width, height;
    //RefreshRate framRate;

    //전체화면 토글
    public Toggle toggle_fullScreen;
    private FullScreenMode fullScreenMode;

    public Toggle toggle_Vsync;
    public TMP_Dropdown dropdown_AntiAliasing;
    public TMP_Dropdown dropdown_Quality;
    public Slider slider_Brightness;
    public Slider slider_FPS;
    
    public void Active(bool b)
    {
        this.gameObject.SetActive(b);

        if (b)
        {
            InitResoultion();
            InitQualityOption();
        }
        else
        {
            ReturnSetting();
        }
    }

    public void Apply()
    {
        Debug.LogError("GraphicSetting 변경사항 저장");

        //화면 비율 설정
        //해상도 설정
        if (resolutions.Count > 0)
        {
            _dropdownResolutionNum = dropdown_resolution.value;

            if (dropdown_resolution.options[_dropdownResolutionNum].text.Contains("Custom"))
            {
                Debug.LogWarning("현재 사용자 설정을 선택중입니다.");
            }
            else
            //Debug.LogWarning($"{resolutions[_dropdownResolutionNum].width} =? {width}");
            //Debug.LogWarning($"{resolutions[_dropdownResolutionNum].height} =? {height}");
            //Debug.LogWarning($"{Mathf.CeilToInt(resolutions[_dropdownResolutionNum].refreshRateRatio.numerator * 0.001f)} =? {framRate}");
            //Debug.LogWarning($"{Screen.fullScreenMode} =? {fullScreenMode}");
            if ((resolutions[_dropdownResolutionNum].width != width) ||
                (resolutions[_dropdownResolutionNum].height != height) ||
                //!(resolutions[_dropdownResolutionNum].refreshRateRatio.Equals(framRate)) ||
                !(Screen.fullScreenMode.Equals(fullScreenMode)))//선택한 것과 현재 해상도가 다르다면?
            {
                //현재 선택된 번호와 실제 해상도 번호와 다른경우 변경
                width = resolutions[_dropdownResolutionNum].width;
                height = resolutions[_dropdownResolutionNum].height;
                //framRate = resolutions[_dropdownResolutionNum].refreshRateRatio;

                //적용 해상도와 전체화면 여부
                Screen.SetResolution(width, height, fullScreenMode);
                //프레임 수 변경
                Application.targetFrameRate = Mathf.CeilToInt((float)(Screen.currentResolution.refreshRateRatio.value));

                Debug.LogWarning("해상도 변경 완료");
            }
        }

        GraphicSetValue graphicSet = new GraphicSetValue();

        //그래픽 퀄리티 변경
        QualitySettings.SetQualityLevel(dropdown_Quality.value, false);
        graphicSet.Quality = QualitySettings.GetQualityLevel();

        //안티앨리어싱 설정
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        graphicSet.AntiAliasing = Mathf.CeilToInt((Mathf.Pow(2, dropdown_AntiAliasing.value+1)));
        urpAsset.msaaSampleCount = graphicSet.AntiAliasing;

        //V 싱크
        QualitySettings.vSyncCount = toggle_Vsync.isOn ? 1 : 0;
        graphicSet.VSync = QualitySettings.vSyncCount;

        //밝기
        graphicSet.Brightness = slider_Brightness.value;

        //FPS
        graphicSet.FPS = Mathf.Clamp((int)slider_FPS.value,30, 240);
        Application.targetFrameRate = graphicSet.FPS;

        DataSet.Instance.GraphicSetValue = graphicSet;
        DataSet.Instance.Save();
    }

    //해상도 드롭다운바에 값 읽어오기
    void InitResoultion()
    {
        if (!GameConfig.IsPc()) return;

        fullScreenMode = Screen.fullScreenMode;
        toggle_fullScreen.isOn = fullScreenMode == FullScreenMode.FullScreenWindow;

        //해상도 값들을 저장
        resolutions.Clear();
        foreach (Resolution resol in Screen.resolutions)
        {
            //Debug.Log($"{resol.width} x {resol.height} [{Mathf.CeilToInt(resol.refreshRateRatio.numerator * 0.001f)}FPS]");

            int fps = Mathf.CeilToInt((float)resol.refreshRateRatio.value);
            //fps 60 제한
            if (fps > 55 && fps < 65)
            {
                //16 : 9 해상도만 가져오기
                float ratio = resol.width / (float)resol.height;
                float ratioCompare = 1.7777777f;
                if ((ratio > (ratioCompare - 0.08f)) && (ratio < (ratioCompare + 0.08f)))
                {
                    resolutions.Add(resol);
                }
            }
        }

        width = -1;
        dropdown_resolution.options.Clear();//기존 옵션 내용 제거
        //드롭다운 박스에 해상도 값을 표시
        int _num = -1;
        foreach (Resolution resolution in resolutions)
        {
            _num++;

            //옵션값을 지정한다.
            TMP_Dropdown.OptionData _option = new TMP_Dropdown.OptionData();
            _option.text = string.Format($"{resolution.width} x {resolution.height}");// [{Mathf.CeilToInt(resolution.refreshRateRatio.numerator * 0.001f)}]"); //[{Mathf.CeilToInt(resolution.refreshRateRatio.numerator * 0.001f)}FPS]");
            dropdown_resolution.options.Add(_option);

            //int fps = Mathf.CeilToInt((float)(Screen.currentResolution.refreshRateRatio.value));
            //현재 옵션값을 판단하여 현재의 해상도를 표시한다.
            if (Screen.width.Equals(resolution.width) &&
                Screen.height.Equals(resolution.height)
                //&& Application.targetFrameRate.Equals(fps)
                //&&Screen.currentResolution.refreshRateRatio.Equals(resolution.refreshRateRatio)
                )
            {
                //아래 값들은 상황판단 변수임
                dropdown_resolution.value = _num;
                _dropdownResolutionNum = _num;

                width = resolution.width;
                height = resolution.height;

                Debug.LogWarning($"Setting : 현재 대응하는 해상도가 존재합니다. {Screen.width} x {Screen.height} [{Application.targetFrameRate}]");
            }
        }

        //선택된 것이 없을 경우
        if(width.Equals(-1))
        {
            //옵션값을 지정한다.
            TMP_Dropdown.OptionData _option = new TMP_Dropdown.OptionData();
            _option.text = $"Custom [{((Mathf.CeilToInt(Application.targetFrameRate)==-1) ? "Infinite" : Mathf.CeilToInt(Application.targetFrameRate))}]FPS]";
            dropdown_resolution.options.Add(_option);

            _dropdownResolutionNum = dropdown_resolution.options.Count - 1;
            dropdown_resolution.value = _dropdownResolutionNum;

            Debug.LogWarning($"Setting : 현재 대응하는 해상도가 없어 Custom으로 변경 {Screen.width} x {Screen.height} [{Application.targetFrameRate}]");
        }

        //새로고침
        dropdown_resolution.RefreshShownValue();

        //FPS 설정
        slider_FPS.value = Mathf.Clamp(Application.targetFrameRate, 0, 240);
    }

    //보여지는 정보를 초기화합니다.
    void InitQualityOption()
    {
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        toggle_Vsync.isOn = (QualitySettings.vSyncCount > 0);
        dropdown_AntiAliasing.value = urpAsset.msaaSampleCount == 2 ? 0 : (urpAsset.msaaSampleCount == 4) ? 1 : (urpAsset.msaaSampleCount == 8) ? 2 : 3;
        dropdown_Quality.value = QualitySettings.GetQualityLevel();
        slider_Brightness.value = DataSet.Instance.GraphicSetValue.Brightness;
    }

    //저장되지 않은 값은 이전 값으로 변경
    void ReturnSetting()
    {
        if (CanBright()) {
            RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor(); 
        }
    }

    //밝기 설정
    public void SetBrightness()
    {
        //카메라를 장착하고 있으면 변경되어서는 안됨.
        if (CanBright()) RenderSettings.ambientLight = slider_Brightness.value * DataSet.Instance.Color_Default;
    }

    bool CanBright()
    {
        //카메라를 장착하고 있으면 변경되어서는 안됨.
        if (PlayerEvent.instance != null)
        {
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

    public void FullScreenToggle()
    {
        fullScreenMode = toggle_fullScreen.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }
}
