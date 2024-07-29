using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

/// <summary>
/// 정적 UI를 컨트롤한다.
/// </summary>

namespace CustomUI
{
    public struct LineText
    {
        public string text;
        public float timeout;
    }
    public class StaticUI : MonoBehaviour
    {
        [Space]
        [Header("게임 내 및에서 나올 대사")]
        [SerializeField] private TMP_Text list_Txt;
        [SerializeField] CanvasGroup line_Group;
        private List<LineText> line_List = new List<LineText>();
        private IEnumerator showLineCroutine;

        private CameraBattery _cameraBattery;

        private void Awake()
        {
            UI.staticUI = this;

            line_Group.gameObject.SetActive(false);

            _cameraBattery = GetComponentInChildren<CameraBattery>();
        }
        private void Start()
        {
            PlayerEvent.instance.Action_Init += LineInit;
        }

        public void SetBattery(float charge, float damged)
        {
            if(_cameraBattery.gameObject.activeSelf)
            {
                _cameraBattery.SetBattery(charge, damged);
            }
        }

        public void EnterLine(string txt, float timeout, bool _short = false)
        {
            //최소 표시 시간 제한
            if ((timeout < 7.0f) && !_short) timeout = 7.0f;

            line_List.Add(new LineText { text = $"\"{txt}\"", timeout = timeout });

        }
        public void ShowLine()
        {
            if(showLineCroutine != null) { StopCoroutine(showLineCroutine); }
            showLineCroutine = ShowLineCroutine();
            StartCoroutine(showLineCroutine);
        }

        IEnumerator ShowLineCroutine()
        {
            line_Group.gameObject.SetActive(true);
            LineText[] lineTexts = line_List.ToArray();
            line_List.Clear();

            line_Group.alpha = 1.0f;

            foreach (LineText lineText in lineTexts)
            {
                float lineTimeout = 0;
                list_Txt.text = lineText.text;

                while (lineTimeout < lineText.timeout)
                {
                    lineTimeout += Time.deltaTime;
                    yield return null;
                }
            }

            float ratio = 1.0f;
            while(ratio > 0.0f)
            {
                ratio -= Time.deltaTime;
                line_Group.alpha = ratio;
                yield return null;
            }
            line_Group.alpha = 0.0f;

            showLineCroutine = null;
            line_Group.gameObject.SetActive(false);
        }

        //대사를 끈다.
        public void LineInit()
        {
            line_Group.alpha = 0.0f;

            showLineCroutine = null;
            line_Group.gameObject.SetActive(false);
        }
    }
}