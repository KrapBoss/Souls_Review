using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
/// 그래픽을 
/// </summary>
public class OtherSetting : MonoBehaviour, Setter
{
    //소리
    public Slider slider_BGM;
    public Slider slider_Effect;
    //감도
    public Slider slider_Sensitivity;

    public void Active(bool b)
    {
        this.gameObject.SetActive(b);

        if (b)
        {
            Init();
        }
    }

    public void Apply()
    {
        Debug.LogError("OtherSetting 변경사항 저장");

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
        DataSet.Instance.Save();

        //기본음 변경
        foreach (var source in FindObjectsOfType<AudioSource>())
        {
            source.volume = DataSet.Instance.SettingValue.Volume;
        }

        //다른 것들도 변경
        Setting.Action_SoundChanger();
    }

    public void Init() 
    {
        slider_BGM.value = DataSet.Instance.SettingValue.BGMVoulme;
        slider_Effect.value = DataSet.Instance.SettingValue.Volume;
        slider_Sensitivity.value = DataSet.Instance.SettingValue.MouseSensitivity;
    }
}
