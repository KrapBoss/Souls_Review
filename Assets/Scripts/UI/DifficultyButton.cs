
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    public Button button;
    public TMP_Text text_Timer;
    public GameDifficulty[] Diff;///���̵�
    public LocalizeStringEvent localizeString_Button;
    public LocalizeStringEvent localizeString_TextArea;

    float timeOut;

    //���� ���̵�
    public int index = 1;

    private IEnumerator Start()
    {
        button.onClick.AddListener(OnClickButton);

        //���̵� �ε�
        index = DataSet.Instance.SettingValue.CurrentDiff;
        DataSet.Instance.GameDifficulty = Diff[index];

        //������ �� ã���ֱ�
        while (true)
        {
            if (LocalLanguageSetting.Instance.LoadEnd()) break;
            yield return null;
        }

        ////���� �����Ǿ� �ִ� ���̵��� ��ư �ؽ�Ʈ ����
        localizeString_Button.SetEntry($"{GetKey(index)}Button");
        localizeString_TextArea.SetEntry($"{GetKey(index)}Description");


        float clearTime = DataSet.Instance.SettingValue.ClearTime[index];
        float hour = clearTime / 3600;
        float minute = clearTime % 3600 / 60;
        float second = clearTime % 3600 % 60;

        //Ŭ���� �ð� ����
        text_Timer.text = $"{string.Format("{0:00}", hour)} : {string.Format("{0:00}", minute)} : {string.Format("{0:00.#}",second)}";
    }


    private void Update()
    {
        if(timeOut > 0)
        {
            timeOut -= Time.deltaTime;
        }
    }

    public void OnClickButton()
    {
        if (timeOut > 0) return;

        timeOut = 0.1f;

        //���̵� ����
        index = (index + 1) % 3; 
        DataSet.Instance.SettingValue.CurrentDiff = index;
        DataSet.Instance.GameDifficulty = Diff[index];

        //Ÿ�̸� ����
        float clearTime = DataSet.Instance.SettingValue.ClearTime[index];
        float hour = clearTime / 3600;
        float minute = clearTime % 3600 / 60;
        float second = clearTime % 3600 % 60;
        text_Timer.text = $"{string.Format("{0:00}", hour)} : {string.Format("{0:00}", minute)} : {string.Format("{0:00.#}", second)}";

        //���� ����
        localizeString_Button.StringReference.TableEntryReference = $"{GetKey(index)}Button";
        localizeString_TextArea.StringReference.TableEntryReference = $"{GetKey(index)}Description";

        DataSet.Instance.Save();
    }

    string GetKey(int index)
    {
        switch (index)
        {
            case 0://����
                return "Easy_";
            case 1://����
                return "Normal_";
            case 2://�����
                return "Hard_";
            default:
                return "Normal_";
        }
    }
}