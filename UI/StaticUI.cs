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
        public DialogManager dialogueManager;
        private List<LineText> line_List = new List<LineText>();

        private CameraBattery _cameraBattery;

        private void Awake()
        {
            UI.staticUI = this;

            _cameraBattery = GetComponentInChildren<CameraBattery>();
        }
        private void Start()
        {
            PlayerEvent.instance.Action_Init += LineInit;
        }

        private void OnDestroy()
        {
            PlayerEvent.instance.Action_Init -= LineInit;
        }

        public void SetBattery(float charge, float damged)
        {
            if(_cameraBattery.gameObject.activeSelf)
            {
                _cameraBattery.SetBattery(charge, damged);
            }
        }

        /// <summary> 지문 저장 </summary>
        /// <param name="txt"> 지문 </param>
        /// <param name="timeout"> 지속 시간 </param>
        public void EnterLine(string txt, float timeout)
        {
            //최소 표시 시간 제한
            if (timeout < 7.0f) timeout = 7.0f;

            line_List.Add(new LineText { text = $"\"{txt}\"", timeout = timeout });
        }

        /// <summary> 지금까지 저장한 모든 지문을 보여줍니다. </summary>
        public void ShowLine()
        {
            //if(PlayerEvent.instance.isDead) return;

            int index = 0;
            foreach (LineText lineText in line_List)
            {
                dialogueManager.AddDialogue(lineText.text, lineText.timeout, lineText.timeout * 0.5f * index);
                index++;
            }

            line_List.Clear();
        }

        //대사를 끈다.
        public void LineInit()
        {
            line_List.Clear();
            dialogueManager.AllDisappear();
        }
    }
}