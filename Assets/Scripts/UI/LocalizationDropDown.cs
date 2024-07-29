using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

public class LocalizationDropDown : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private void Start()
    {
        //드롭다운 선택 이벤트 추가
        dropdown.onValueChanged.AddListener(LocaleSelected);
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        // Wait for the localization system to initialize, loading Locales, preloading etc.
        yield return LocalizationSettings.InitializationOperation;

        // Generate list of available Locales
        var options = new List<TMP_Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
            options.Add(new TMP_Dropdown.OptionData(locale.name));
        }

        dropdown.options = options;

        dropdown.value = selected;

        dropdown.RefreshShownValue();//새로고침
    }

    static void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
