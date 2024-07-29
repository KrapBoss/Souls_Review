using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
/// �׷����� 
/// </summary>
public class OtherSetting : MonoBehaviour, Setter
{
    //�Ҹ�
    public Slider slider_BGM;
    public Slider slider_Effect;
    //����
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
        Debug.LogError("OtherSetting ������� ����");

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
        DataSet.Instance.Save();

        //�⺻�� ����
        foreach (var source in FindObjectsOfType<AudioSource>())
        {
            source.volume = DataSet.Instance.SettingValue.Volume;
        }

        //�ٸ� �͵鵵 ����
        Setting.Action_SoundChanger();
    }

    public void Init() 
    {
        slider_BGM.value = DataSet.Instance.SettingValue.BGMVoulme;
        slider_Effect.value = DataSet.Instance.SettingValue.Volume;
        slider_Sensitivity.value = DataSet.Instance.SettingValue.MouseSensitivity;
    }
}
