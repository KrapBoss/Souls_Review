
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
    public GameDifficulty[] Diff;///난이도
    public LocalizeStringEvent localizeString_Button;
    public LocalizeStringEvent localizeString_TextArea;

    float timeOut;

    //현재 난이도
    public int index = 1;

    private IEnumerator Start()
    {
        button.onClick.AddListener(OnClickButton);

        //난이도 로드
        index = DataSet.Instance.SettingValue.CurrentDiff;
        DataSet.Instance.GameDifficulty = Diff[index];

        //딜레이 후 찾아주기
        while (true)
        {
            if (LocalLanguageSetting.Instance.LoadEnd()) break;
            yield return null;
        }

        ////현재 지정되어 있는 난이도로 버튼 텍스트 설정
        localizeString_Button.SetEntry($"{GetKey(index)}Button");
        localizeString_TextArea.SetEntry($"{GetKey(index)}Description");


        float clearTime = DataSet.Instance.SettingValue.ClearTime[index];
        float hour = clearTime / 3600;
        float minute = clearTime % 3600 / 60;
        float second = clearTime % 3600 % 60;

        //클리어 시간 지정
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

        //난이도 지정
        index = (index + 1) % 3; 
        DataSet.Instance.SettingValue.CurrentDiff = index;
        DataSet.Instance.GameDifficulty = Diff[index];

        //타이머 변경
        float clearTime = DataSet.Instance.SettingValue.ClearTime[index];
        float hour = clearTime / 3600;
        float minute = clearTime % 3600 / 60;
        float second = clearTime % 3600 % 60;
        text_Timer.text = $"{string.Format("{0:00}", hour)} : {string.Format("{0:00}", minute)} : {string.Format("{0:00.#}", second)}";

        //참조 변경
        localizeString_Button.StringReference.TableEntryReference = $"{GetKey(index)}Button";
        localizeString_TextArea.StringReference.TableEntryReference = $"{GetKey(index)}Description";

        DataSet.Instance.Save();
    }

    string GetKey(int index)
    {
        switch (index)
        {
            case 0://쉬움
                return "Easy_";
            case 1://보통
                return "Normal_";
            case 2://어려움
                return "Hard_";
            default:
                return "Normal_";
        }
    }
}