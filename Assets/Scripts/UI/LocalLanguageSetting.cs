using CustomUI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

/// <summary>
/// �������� ������ ������ ���
/// </summary>
[System.Serializable]
public class ItemShowText{

    [Header("�ݺ� Ƚ��")]
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

//�� ������ �ٲ��ش�.
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

    //�� �����մϴ�
    public void ChangeLocale(int index)
    {
        if (isChanging)
        {
            return;
        }

        CoroutineHandler.StartCorou(ChageCoroutine(index));
    }

    //��� ����
    IEnumerator ChageCoroutine(int index)
    {
        isChanging = true;

        if (LocalizationSettings.AvailableLocales.Locales.Count < index)
        {
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        }

        //���� ����� �̸��� �����ɴϴ�.
        GameConfig.currentLanguage = LocalizationSettings.SelectedLocale.LocaleName;
        isChanging = false;

        //�ڷ�ƾ ���� ���� ����
        yield return -1;
    }

    //�ش��ϴ� ���̺��� �� ���ɴϴ�.
    public string GetLocalText(string table, string key)
    {
        string txt = "";

        //������� ��� �ش� ����� Ű�� �����ϴ��� �Ǵ��մϴ�.
        if (!GameConfig.IsPc())
        {
            string tmp_key = key + "_Mobile";
            var collection = LocalizationSettings.StringDatabase.GetTableEntry(table,tmp_key);

            string entry = null;
            if (collection.Entry != null)
            {
                entry = collection.Entry.Key;
            }

            //Ű�� �ִ� ���
            if (entry!= null)
            {
                txt = LocalizationSettings.StringDatabase.GetLocalizedString(table, tmp_key);
                return txt;
            }
        }

        //���� key���� �ش��ϴ� �ؽ�Ʈ
        txt = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);

        return txt;
    }

    public bool LoadEnd()
    {
        return LocalizationSettings.InitializationOperation.IsValid();
    }
}
