using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI
{
    public class Semi_StaticUI : MonoBehaviour
    {
        [Space]
        [Header("고스트 미터기에 해당하는 오브젝트를 넣으시오,.")]
        [SerializeField] GameObject Panel_GhostMeter;   //활성비활성시킬 오브젝트
        [SerializeField] Image[] Image_Needles;            //방향을 표시해줄 오브젝트
        [SerializeField] Image image_Roundary;// 미터기 라우ㄴㄷㅓㄹ
        float needleHalfSize;
        float roundaryHalfSizeX;
        float roundaryHalfSizeY;

        [Space]
        [Header("스토리 종이")]
        public GameObject go_paper;
        PaperUI paperUI;

        [Space]
        [Header("잠금")]
        public GameObject go_EnterPassword;
        [Space]
        [Header("전기함")]
        public GameObject go_WireBox;

        [Space]
        [Header("카메라")]
        [SerializeField] Canvas cam_canvas;
        [SerializeField] Image camFlash_image;

        [Space]
        [Header("설명 영상")]
        [SerializeField] ShowItemDescription cs_ItemDescription;


        //Get Screen Size
        float screen_yHalf;

        bool ghostMeter;

        private void Awake()
        {
            UI.semiStaticUI = this;
        }

        private void Start()
        {
            EventManager.instance.Action_CameraEquip += CameraEquip;

            EventManager.instance.Action_CameraFlash += CameraFlash;

            //카메라 오브젝트 초기화
            cam_canvas.enabled = false;

            GhostMeterEquip(false);

            screen_yHalf = Screen.height * 0.5f;

            roundaryHalfSizeX = image_Roundary.rectTransform.rect.width * 0.4f;
            roundaryHalfSizeY = image_Roundary.rectTransform.rect.height * 0.5f;
            needleHalfSize = Image_Needles[0].rectTransform.rect.width * 0.5f;

            go_EnterPassword.SetActive(false);

            cs_ItemDescription.gameObject.SetActive(false);

            paperUI = go_paper.GetComponent<PaperUI>();
            ShowPaper(null);
        }

        private void Update()
        {
            if (ghostMeter)
            {
                Color c;
                for (int i = 0; i < Image_Needles.Length; i++)
                {
                    if ((Image_Needles[i].enabled) && (Image_Needles[i].color.a > 0.001f))
                    {
                        c = Image_Needles[i].color;
                        c.a -= Time.deltaTime * 0.5f;
                        Image_Needles[i].color = c;
                    }
                }
            }
        }

        //아이템에 대한 설명을 보여줍니다.
        public void ShowVideo(VideoType type)
        {
            cs_ItemDescription.gameObject.SetActive (true);
            cs_ItemDescription.Show(type);
        }

        //카메라 장착 시 배경 전환과 UI 표시
        public void CameraEquip(bool _equip)
        {
            if (_equip)
            {
                RenderSettings.ambientLight = DataSet.Instance.Color_Camera * (IntroSystem.Active ? 1.5f : 3.0f);
                cam_canvas.enabled = (true);
                Color c = camFlash_image.color;
                c.a = 0;
                camFlash_image.color = c;
            }
            else
            {
                //튜토리얼이 종료된 후에 변경이 가능합니다.
                if (IntroSystem.Active)
                    RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor();
                else
                    RenderSettings.ambientLight = DataSet.Instance.Color_Evening;


                CancelInvoke("BatteryUsing");
                cam_canvas.enabled = (false);
            }
        }


        bool isflashing;
        //카메라 사진 찍기
        public void CameraFlash()
        {
            if (isflashing) return;
            StartCoroutine(CameraFlashCroutine());
        }

        IEnumerator CameraFlashCroutine() //사진을 찍으면 플래시를 조절한다.
        {
            Debug.Log("플래시를 촬영합니다.");
            isflashing = true;
            float ratio = 1;
            Color c = camFlash_image.color;
            c.a = 0;
            camFlash_image.color = c;

            for (int i = 1; i <= 2; i++)
            {
                ratio = i * 0.5f;
                while (ratio > 0.0f)
                {
                    c.a = ratio;
                    camFlash_image.color = c;

                    ratio -= Time.deltaTime * 3.5f;

                    yield return null;

                }
            }

            c.a = 0;
            camFlash_image.color = c;

            isflashing = false;
        }

        //고스트 미터기 키고 끄기
        public void GhostMeterEquip(bool b)
        {
            Panel_GhostMeter.SetActive(b);

            ghostMeter = b;

            if (!b)
            {
                for (int i = 0; i < Image_Needles.Length; i++) { Image_Needles[i].enabled = false; }
            }
        }

        float DegreeToRadian = Mathf.Deg2Rad; //=> PI/ 180

        public void GhostMeterDirection(List<float> dics)   //방향을 표시해줄 오브젝트
        {
            //Debug.Log("Semi_StaticUI :: GhostMeterDirection");
            Vector2 _vector = Vector2.zero;
            int i = 0;
            for (i = 0; (i < Image_Needles.Length) && (dics.Count > i); i++)
            {

                //방향표시 오브젝트를 켜주고 바늘의 투명도를 조절한다.
                Image_Needles[i].enabled = true;
                Color c = Image_Needles[i].color;
                c.a = 1;
                Image_Needles[i].color = c;

                //넘겨온 방향에 따른 위치 값을 계산함. cos를 y로 사용하여 forward를 표현
                float x = Mathf.Sin(dics[i] * DegreeToRadian) * roundaryHalfSizeX;// * screen_xHalf * 2;
                float y = Mathf.Cos(dics[i] * DegreeToRadian) * roundaryHalfSizeY;// * screen_yHalf * 2;

                //x = Mathf.Clamp(x, -screen_xHalf, screen_xHalf);
                y = Mathf.Clamp(y, -screen_yHalf, screen_yHalf);

                x = (x > 0) ? x - needleHalfSize : x + needleHalfSize;
                y = (y > 0) ? y - needleHalfSize : y + needleHalfSize;

                _vector.Set(x, y);
                Image_Needles[i].rectTransform.anchoredPosition = _vector;
            }

            //남아있는 이미지를 끈다.
            for (; i < Image_Needles.Length; i++) { Image_Needles[i].enabled = false; }
        }


        //For Paper
        public void ShowPaper(string _content)
        {
            if (_content != null)
            {
                go_paper.SetActive(true);
                paperUI.SetPaper(_content);
            }
            else
            {
                go_paper.SetActive(false);
            }
        }


        //For SafeBox
        Action<int[]> action_enterPassword;
        public void ShowEnterPassword(Action<int[]> _action)//패스워드 UI를 보여준다.
        {
            GameManager.Instance.FrozenOnGame(FrozenOffSafeBox);

            go_EnterPassword.SetActive(true);
            action_enterPassword = _action;

            GameManager.Instance.CursorShow();
        }
        public void DecideEnterPassword(int[] pw)   //패스워드를 사용자가 선택했을 경우 넘겨준다.
        {
            action_enterPassword(pw);

            GameManager.Instance.FrozenOffGame();
        }
        public void HideEnterPassword()             //숨기기
        {
            GameManager.Instance.FrozenOffGame();
        }
        public void FrozenOffSafeBox()
        {
            go_EnterPassword.SetActive(false);

            action_enterPassword(new int[2] { -1, -1});
            action_enterPassword = null;

            GameManager.Instance.CursorHide();
        }


        //전기함을 위함
        public void WireBox(Action<bool> action)
        {
            //활성화시킵니다.
            go_WireBox.SetActive(true);
            go_WireBox.GetComponent<WireBox>().SetEvent(action);

            //게임을 정지시키는데, ESC를 누르면 전기함이 꺼진다.
            GameManager.Instance.FrozenOnGame(() => { go_WireBox.GetComponent<WireBox>().Close(); });
        }
    }
}