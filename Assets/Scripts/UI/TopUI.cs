using JetBrains.Annotations;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// ���� ���� ǥ�õǾ�� �Ǵ� UI�� �̰����� ����Ѵ�.
namespace CustomUI{
    public class TopUI : MonoBehaviour
    {
        [Space]
        [Header("Dazzle")]
        [SerializeField] Image image_dazzle;
        [SerializeField] float dazzle_maxScale;
        float dazzle_alpha = 1.0f;

        [Space]
        [Header("Setting")]
        public Canvas setting_Canvas;
        public Canvas setting_CanvasMobile;

        [Space]
        [Header("Notice is Goal to taking a game")]
        [SerializeField] TMP_Text notice_text;
        [SerializeField] CanvasGroup notice_group;
        //SerializeField] Button notice_button;


        [Space]
        [Header("No Signal Panel")]
        [SerializeField]NoSignal NoSignal;

        // Start is called before the first frame update
        void Start()
        {
            UI.topUI = this;

            image_dazzle.enabled = false;

            if (GameConfig.IsPc())
            {
                Destroy(setting_CanvasMobile.gameObject);
                setting_Canvas.enabled = false;
            }
            else
            {
                Destroy(setting_Canvas.gameObject);
                setting_CanvasMobile.enabled = false;
            }

            PlayerEvent.instance.Action_Init += NoticeInit;

            notice_group.alpha = 0.0f;
            notice_text.color = Color.white;
            notice_text.enabled = false;
            notice_group.gameObject.SetActive(false);

            /*
            notice_button.gameObject.SetActive(false);
            if (!GameConfig.IsPc())
            {
                notice_button.onClick.AddListener(ReShowNotice);
            }*/
        }

        #region Dazzle

        IEnumerator dazzleCroutine = null;
        //�ִ�� ��� �� �� �����ð� / ������� �ð�
        public void Dazzle(float _keepTime, float _fadeOutTime)
        {
            image_dazzle.enabled = true;

            if(dazzleCroutine != null ) { return; }

            dazzleCroutine = DazzleCroutine(_keepTime, _fadeOutTime);
            StartCoroutine(dazzleCroutine);
        }
        IEnumerator DazzleCroutine(float _keepTime, float _fadeOutTime)
        {
            float _t = 0.001f;
            Color _color = image_dazzle.color;

            //�̹��� ������ Ű���, ���İ� �ø���
            while (_t < 1.0f)
            {
                //���� ����
                _color.a = Mathf.Lerp(0, dazzle_alpha, _t);
                image_dazzle.color = _color;

                //������ ����
                float _scale = Mathf.Lerp(0, dazzle_maxScale, _t);
                image_dazzle.rectTransform.localScale = new Vector3(_scale, _scale,1);

                //�ð� ����
                _t += Time.unscaledDeltaTime * 10;
                yield return null;
            }
            _color.a = dazzle_alpha;
            image_dazzle.color = _color;
            image_dazzle.rectTransform.localScale = new Vector3(dazzle_maxScale, dazzle_maxScale, 1);


            ////�����ð� ���� ��Ű��
            yield return new WaitForSeconds(_keepTime);


            //////���İ��� ������ ���� ���δ�.
            float _timeRatio = 0; //�����ð����� 1�� ������ ������ ��
            _t = 0.0001f;
            
            while (_t < _fadeOutTime) {

                _timeRatio = _t/ _fadeOutTime;

                if(_timeRatio > 0.33f)//���� ������ �����ٸ�,
                {
                    //���İ��� ���δ�.
                    _color.a = Mathf.Lerp(dazzle_alpha, 0.0f, _timeRatio);
                    image_dazzle.color = _color;
                }

                //������ ����
                float _scale = Mathf.Lerp(dazzle_maxScale, 0, _timeRatio);
                image_dazzle.rectTransform.localScale = new Vector3(dazzle_maxScale, _scale, 1);

                _t += Time.unscaledDeltaTime;

                yield return null;
            }

            //��Ȱ��ȭ
            image_dazzle.enabled = false;
            dazzleCroutine = null;
        }
        #endregion

        ///Setting UI
        public void ShowSetting()
        {
            AudioManager.instance.PlayUISound("Check", 1.0f);
            
            //Window �� ���
            if (GameConfig.IsPc())
            {
                setting_Canvas.enabled = (true);
                setting_Canvas.GetComponent<InGameSetting>().Init();
            }
            else
            {
                setting_CanvasMobile.enabled = (true);
                setting_CanvasMobile.GetComponent<InGameSetting>().Init();
            }

            
            GameManager.Instance.CursorShow();
            GameManager.Instance.FrozenOnGame(HideSetting);
        }

        public void HideSetting()
        {
            //Window �� ���
            if (GameConfig.IsPc())
            {
                setting_Canvas.enabled = (false);
            }
            else
            {
                setting_CanvasMobile.enabled = (false);
            }

            GameManager.Instance.CursorHide();
        }






        //-------------��ǥ

        IEnumerator m_ShowNoticeCroutine;
        string note = null;

        //For Notice
        public void ShowNotice(string _note,bool isSave, float time = 5.0f)
        {
            if (!notice_group.gameObject.activeSelf) notice_group.gameObject.SetActive(true);

            if (isSave)
            {
                AudioManager.instance.PlayUISound("Hint", 1.0f);
            }

            if (m_ShowNoticeCroutine != null)//���� �̹� ��Ƽ���� �۵� ���̶�� �����Ѵ�.
            {
                StopCoroutine(m_ShowNoticeCroutine);
                m_ShowNoticeCroutine = null;
            }

            //notice_button.gameObject.SetActive(false);

            if (isSave) { note = _note; }
            
            m_ShowNoticeCroutine = ShowNoticeCroutine(_note, time);
            StartCoroutine(m_ShowNoticeCroutine);
        }



        IEnumerator ShowNoticeCroutine(string _note, float time)//���� �ð��� ���� �� �����ϰ� �����Ѵ�.
        {
            //���� �� �ؽ��� �ʱⰪ�� �����ϴ� ��
            notice_text.enabled = true;
            notice_text.text = string.Format($"[{_note}]");
            yield return ShowNoticeFadeCroutine(true);

            yield return new WaitForSeconds(time);

            yield return ShowNoticeFadeCroutine(false);

            //notice_text.enabled = false;

            //�����
            //if(note !=null && !GameConfig.IsPc())notice_button.gameObject.SetActive(true);

            notice_group.gameObject.SetActive(false);
            m_ShowNoticeCroutine = null;
        }

        //��ǥ�� �ٽ� �����ݴϴ�.
        public void ReShowNotice()
        {
            if(note != null)
            {
                ShowNotice(note, false);
            }
        }


        IEnumerator ShowNoticeFadeCroutine(bool _bool) // NoticeUI Fade
        {
            float t = 0;
            float _alpha;
            float targetAlpha;

            if (_bool)// ���̵� ��
            {
                _alpha = 0.0f;
                targetAlpha = 1.0f;
            }
            else//���̵� �ƿ�
            {
                _alpha = 1.0f;
                targetAlpha = 0.0f;
            }

            while (t< 1.0f)
            {
                t += Time.unscaledDeltaTime;
                _alpha = Mathf.Lerp(_alpha, targetAlpha, t);
                notice_group.alpha = _alpha;
                yield return null;
            }

            notice_group.alpha = targetAlpha;
        }

        //�⺻ ����
        public void NoticeInit() { 
            if(m_ShowNoticeCroutine != null)
            {
                StopAllCoroutines();
                //notice_text.enabled = false;
                notice_group.gameObject.SetActive(false);
                m_ShowNoticeCroutine = null;

                //if(note != null && !GameConfig.IsPc())notice_button.gameObject.SetActive(true);
            }
        }
        //----------end ��ǥ


        public void ShowNoSignal()
        {
            NoSignal.Show();
        }

        [Header("������ ��ȸ"),Space]
        [SerializeField] LastChanceAds_Mobile LastChanceAds_Mobile;
        public void LastChance(UnityAction yes, UnityAction no)
        {
            LastChanceAds_Mobile.gameObject.SetActive(true);
            LastChanceAds_Mobile.Show(yes, no);
        }

        public void LastChanceActive(bool active)
        {
            LastChanceAds_Mobile.gameObject.SetActive(active);
        }
    }
}

