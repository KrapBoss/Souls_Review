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
    List<Resolution> resolutions = new List<Resolution>();//�ػ󵵸� ��´�.
    public int _dropdownResolutionNum = -1; // ��ӹڽ� ������ ������ �ػ� �迭 ��ȣ

    //���� �����Ǿ� �ִ� �ػ� ����
    int width, height;
    //RefreshRate framRate;

    //��üȭ�� ���
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
        Debug.LogError("GraphicSetting ������� ����");

        //ȭ�� ���� ����
        //�ػ� ����
        if (resolutions.Count > 0)
        {
            _dropdownResolutionNum = dropdown_resolution.value;

            if (dropdown_resolution.options[_dropdownResolutionNum].text.Contains("Custom"))
            {
                Debug.LogWarning("���� ����� ������ �������Դϴ�.");
            }
            else
            //Debug.LogWarning($"{resolutions[_dropdownResolutionNum].width} =? {width}");
            //Debug.LogWarning($"{resolutions[_dropdownResolutionNum].height} =? {height}");
            //Debug.LogWarning($"{Mathf.CeilToInt(resolutions[_dropdownResolutionNum].refreshRateRatio.numerator * 0.001f)} =? {framRate}");
            //Debug.LogWarning($"{Screen.fullScreenMode} =? {fullScreenMode}");
            if ((resolutions[_dropdownResolutionNum].width != width) ||
                (resolutions[_dropdownResolutionNum].height != height) ||
                //!(resolutions[_dropdownResolutionNum].refreshRateRatio.Equals(framRate)) ||
                !(Screen.fullScreenMode.Equals(fullScreenMode)))//������ �Ͱ� ���� �ػ󵵰� �ٸ��ٸ�?
            {
                //���� ���õ� ��ȣ�� ���� �ػ� ��ȣ�� �ٸ���� ����
                width = resolutions[_dropdownResolutionNum].width;
                height = resolutions[_dropdownResolutionNum].height;
                //framRate = resolutions[_dropdownResolutionNum].refreshRateRatio;

                //���� �ػ󵵿� ��üȭ�� ����
                Screen.SetResolution(width, height, fullScreenMode);
                //������ �� ����
                Application.targetFrameRate = Mathf.CeilToInt((float)(Screen.currentResolution.refreshRateRatio.value));

                Debug.LogWarning("�ػ� ���� �Ϸ�");
            }
        }

        GraphicSetValue graphicSet = new GraphicSetValue();

        //�׷��� ����Ƽ ����
        QualitySettings.SetQualityLevel(dropdown_Quality.value, false);
        graphicSet.Quality = QualitySettings.GetQualityLevel();

        //��Ƽ�ٸ���� ����
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        graphicSet.AntiAliasing = Mathf.CeilToInt((Mathf.Pow(2, dropdown_AntiAliasing.value+1)));
        urpAsset.msaaSampleCount = graphicSet.AntiAliasing;

        //V ��ũ
        QualitySettings.vSyncCount = toggle_Vsync.isOn ? 1 : 0;
        graphicSet.VSync = QualitySettings.vSyncCount;

        //���
        graphicSet.Brightness = slider_Brightness.value;

        //FPS
        graphicSet.FPS = Mathf.Clamp((int)slider_FPS.value,30, 240);
        Application.targetFrameRate = graphicSet.FPS;

        DataSet.Instance.GraphicSetValue = graphicSet;
        DataSet.Instance.Save();
    }

    //�ػ� ��Ӵٿ�ٿ� �� �о����
    void InitResoultion()
    {
        if (!GameConfig.IsPc()) return;

        fullScreenMode = Screen.fullScreenMode;
        toggle_fullScreen.isOn = fullScreenMode == FullScreenMode.FullScreenWindow;

        //�ػ� ������ ����
        resolutions.Clear();
        foreach (Resolution resol in Screen.resolutions)
        {
            //Debug.Log($"{resol.width} x {resol.height} [{Mathf.CeilToInt(resol.refreshRateRatio.numerator * 0.001f)}FPS]");

            int fps = Mathf.CeilToInt((float)resol.refreshRateRatio.value);
            //fps 60 ����
            if (fps > 55 && fps < 65)
            {
                //16 : 9 �ػ󵵸� ��������
                float ratio = resol.width / (float)resol.height;
                float ratioCompare = 1.7777777f;
                if ((ratio > (ratioCompare - 0.08f)) && (ratio < (ratioCompare + 0.08f)))
                {
                    resolutions.Add(resol);
                }
            }
        }

        width = -1;
        dropdown_resolution.options.Clear();//���� �ɼ� ���� ����
        //��Ӵٿ� �ڽ��� �ػ� ���� ǥ��
        int _num = -1;
        foreach (Resolution resolution in resolutions)
        {
            _num++;

            //�ɼǰ��� �����Ѵ�.
            TMP_Dropdown.OptionData _option = new TMP_Dropdown.OptionData();
            _option.text = string.Format($"{resolution.width} x {resolution.height}");// [{Mathf.CeilToInt(resolution.refreshRateRatio.numerator * 0.001f)}]"); //[{Mathf.CeilToInt(resolution.refreshRateRatio.numerator * 0.001f)}FPS]");
            dropdown_resolution.options.Add(_option);

            //int fps = Mathf.CeilToInt((float)(Screen.currentResolution.refreshRateRatio.value));
            //���� �ɼǰ��� �Ǵ��Ͽ� ������ �ػ󵵸� ǥ���Ѵ�.
            if (Screen.width.Equals(resolution.width) &&
                Screen.height.Equals(resolution.height)
                //&& Application.targetFrameRate.Equals(fps)
                //&&Screen.currentResolution.refreshRateRatio.Equals(resolution.refreshRateRatio)
                )
            {
                //�Ʒ� ������ ��Ȳ�Ǵ� ������
                dropdown_resolution.value = _num;
                _dropdownResolutionNum = _num;

                width = resolution.width;
                height = resolution.height;

                Debug.LogWarning($"Setting : ���� �����ϴ� �ػ󵵰� �����մϴ�. {Screen.width} x {Screen.height} [{Application.targetFrameRate}]");
            }
        }

        //���õ� ���� ���� ���
        if(width.Equals(-1))
        {
            //�ɼǰ��� �����Ѵ�.
            TMP_Dropdown.OptionData _option = new TMP_Dropdown.OptionData();
            _option.text = $"Custom [{((Mathf.CeilToInt(Application.targetFrameRate)==-1) ? "Infinite" : Mathf.CeilToInt(Application.targetFrameRate))}]FPS]";
            dropdown_resolution.options.Add(_option);

            _dropdownResolutionNum = dropdown_resolution.options.Count - 1;
            dropdown_resolution.value = _dropdownResolutionNum;

            Debug.LogWarning($"Setting : ���� �����ϴ� �ػ󵵰� ���� Custom���� ���� {Screen.width} x {Screen.height} [{Application.targetFrameRate}]");
        }

        //���ΰ�ħ
        dropdown_resolution.RefreshShownValue();

        //FPS ����
        slider_FPS.value = Mathf.Clamp(Application.targetFrameRate, 0, 240);
    }

    //�������� ������ �ʱ�ȭ�մϴ�.
    void InitQualityOption()
    {
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        toggle_Vsync.isOn = (QualitySettings.vSyncCount > 0);
        dropdown_AntiAliasing.value = urpAsset.msaaSampleCount == 2 ? 0 : (urpAsset.msaaSampleCount == 4) ? 1 : (urpAsset.msaaSampleCount == 8) ? 2 : 3;
        dropdown_Quality.value = QualitySettings.GetQualityLevel();
        slider_Brightness.value = DataSet.Instance.GraphicSetValue.Brightness;
    }

    //������� ���� ���� ���� ������ ����
    void ReturnSetting()
    {
        if (CanBright()) {
            RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor(); 
        }
    }

    //��� ����
    public void SetBrightness()
    {
        //ī�޶� �����ϰ� ������ ����Ǿ�� �ȵ�.
        if (CanBright()) RenderSettings.ambientLight = slider_Brightness.value * DataSet.Instance.Color_Default;
    }

    bool CanBright()
    {
        //ī�޶� �����ϰ� ������ ����Ǿ�� �ȵ�.
        if (PlayerEvent.instance != null)
        {
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

    public void FullScreenToggle()
    {
        fullScreenMode = toggle_fullScreen.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }
}
