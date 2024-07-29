using CustomUI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

/// <summary>
/// 아이템을 잡으면 보여줄 대사
/// </summary>
[System.Serializable]
public class ItemShowText{

    [Header("반복 횟수")]
    public short iterator = 1;
    short m_iterator = 0;

    [Header("Notice")]
    public string table = "";
    public string key = "";

    [Header("Line")]
    public string table_Line = "";
    public string key_Line = "";

    public void ShowText()
    {
        if (m_iterator < iterator)
        {
            m_iterator++;
            if (!table.Equals(""))
            {
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText(table, key), false);
            }

            if (!table_Line.Equals(""))
            {
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText(table_Line, key_Line), 6.0f);
                UI.staticUI.ShowLine();
            }
        }
    }
}

//언어를 얻어오고 바꿔준다.
public class LocalLanguageSetting
{
    bool isChanging;

    static LocalLanguageSetting _instance;
    public static LocalLanguageSetting Instance
    {
        get
        {
            if (_instance == null) _instance = new LocalLanguageSetting();
            return _instance;
        }
    }

    //언어를 변경합니다
    public void ChangeLocale(int index)
    {
        if (isChanging)
        {
            return;
        }

        CoroutineHandler.StartCorou(ChageCoroutine(index));
    }

    //언어 변경
    IEnumerator ChageCoroutine(int index)
    {
        isChanging = true;

        if (LocalizationSettings.AvailableLocales.Locales.Count < index)
        {
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        }

        //현재 변경된 이름을 가져옵니다.
        GameConfig.currentLanguage = LocalizationSettings.SelectedLocale.LocaleName;
        isChanging = false;

        //코루틴 상태 종료 조건
        yield return -1;
    }

    //해당하는 테이블의 언어를 얻어옵니다.
    public string GetLocalText(string table, string key)
    {
        string txt = "";

        //모바일일 경우 해당 모바일 키가 존재하는지 판단합니다.
        if (!GameConfig.IsPc())
        {
            string tmp_key = key + "_Mobile";
            var collection = LocalizationSettings.StringDatabase.GetTableEntry(table,tmp_key);

            string entry = null;
            if (collection.Entry != null)
            {
                entry = collection.Entry.Key;
            }

            //키가 있는 경우
            if (entry!= null)
            {
                txt = LocalizationSettings.StringDatabase.GetLocalizedString(table, tmp_key);
                return txt;
            }
        }

        //현재 key값에 해당하는 텍스트
        txt = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);

        return txt;
    }

    public bool LoadEnd()
    {
        return LocalizationSettings.InitializationOperation.IsValid();
    }
}
