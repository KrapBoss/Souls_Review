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
        [Header("��Ʈ ���ͱ⿡ �ش��ϴ� ������Ʈ�� �����ÿ�,.")]
        [SerializeField] GameObject Panel_GhostMeter;   //Ȱ����Ȱ����ų ������Ʈ
        [SerializeField] Image[] Image_Needles;            //������ ǥ������ ������Ʈ
        [SerializeField] Image image_Roundary;// ���ͱ� ��줤���ä�
        float needleHalfSize;
        float roundaryHalfSizeX;
        float roundaryHalfSizeY;

        [Space]
        [Header("���丮 ����")]
        public GameObject go_paper;
        PaperUI paperUI;

        [Space]
        [Header("���")]
        public GameObject go_EnterPassword;
        [Space]
        [Header("������")]
        public GameObject go_WireBox;

        [Space]
        [Header("ī�޶�")]
        [SerializeField] Canvas cam_canvas;
        [SerializeField] Image camFlash_image;

        [Space]
        [Header("���� ����")]
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

            //ī�޶� ������Ʈ �ʱ�ȭ
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

        //�����ۿ� ���� ������ �����ݴϴ�.
        public void ShowVideo(VideoType type)
        {
            cs_ItemDescription.gameObject.SetActive (true);
            cs_ItemDescription.Show(type);
        }

        //ī�޶� ���� �� ��� ��ȯ�� UI ǥ��
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
                //Ʃ�丮���� ����� �Ŀ� ������ �����մϴ�.
                if (IntroSystem.Active)
                    RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor();
                else
                    RenderSettings.ambientLight = DataSet.Instance.Color_Evening;


                CancelInvoke("BatteryUsing");
                cam_canvas.enabled = (false);
            }
        }


        bool isflashing;
        //ī�޶� ���� ���
        public void CameraFlash()
        {
            if (isflashing) return;
            StartCoroutine(CameraFlashCroutine());
        }

        IEnumerator CameraFlashCroutine() //������ ������ �÷��ø� �����Ѵ�.
        {
            Debug.Log("�÷��ø� �Կ��մϴ�.");
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

        //��Ʈ ���ͱ� Ű�� ����
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

        public void GhostMeterDirection(List<float> dics)   //������ ǥ������ ������Ʈ
        {
            //Debug.Log("Semi_StaticUI :: GhostMeterDirection");
            Vector2 _vector = Vector2.zero;
            int i = 0;
            for (i = 0; (i < Image_Needles.Length) && (dics.Count > i); i++)
            {

                //����ǥ�� ������Ʈ�� ���ְ� �ٴ��� ������ �����Ѵ�.
                Image_Needles[i].enabled = true;
                Color c = Image_Needles[i].color;
                c.a = 1;
                Image_Needles[i].color = c;

                //�Ѱܿ� ���⿡ ���� ��ġ ���� �����. cos�� y�� ����Ͽ� forward�� ǥ��
                float x = Mathf.Sin(dics[i] * DegreeToRadian) * roundaryHalfSizeX;// * screen_xHalf * 2;
                float y = Mathf.Cos(dics[i] * DegreeToRadian) * roundaryHalfSizeY;// * screen_yHalf * 2;

                //x = Mathf.Clamp(x, -screen_xHalf, screen_xHalf);
                y = Mathf.Clamp(y, -screen_yHalf, screen_yHalf);

                x = (x > 0) ? x - needleHalfSize : x + needleHalfSize;
                y = (y > 0) ? y - needleHalfSize : y + needleHalfSize;

                _vector.Set(x, y);
                Image_Needles[i].rectTransform.anchoredPosition = _vector;
            }

            //�����ִ� �̹����� ����.
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
        public void ShowEnterPassword(Action<int[]> _action)//�н����� UI�� �����ش�.
        {
            GameManager.Instance.FrozenOnGame(FrozenOffSafeBox);

            go_EnterPassword.SetActive(true);
            action_enterPassword = _action;

            GameManager.Instance.CursorShow();
        }
        public void DecideEnterPassword(int[] pw)   //�н����带 ����ڰ� �������� ��� �Ѱ��ش�.
        {
            action_enterPassword(pw);

            GameManager.Instance.FrozenOffGame();
        }
        public void HideEnterPassword()             //�����
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


        //�������� ����
        public void WireBox(Action<bool> action)
        {
            //Ȱ��ȭ��ŵ�ϴ�.
            go_WireBox.SetActive(true);
            go_WireBox.GetComponent<WireBox>().SetEvent(action);

            //������ ������Ű�µ�, ESC�� ������ �������� ������.
            GameManager.Instance.FrozenOnGame(() => { go_WireBox.GetComponent<WireBox>().Close(); });
        }
    }
}