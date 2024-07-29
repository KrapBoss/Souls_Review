using JetBrains.Annotations;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 가장 위에 표시되어야 되는 UI를 이곳에서 사용한다.
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
        //최대로 밝게 된 후 유지시간 / 사라지는 시간
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

            //이미지 사이즈 키우고, 알파값 올리기
            while (_t < 1.0f)
            {
                //투명도 설정
                _color.a = Mathf.Lerp(0, dazzle_alpha, _t);
                image_dazzle.color = _color;

                //스케일 설정
                float _scale = Mathf.Lerp(0, dazzle_maxScale, _t);
                image_dazzle.rectTransform.localScale = new Vector3(_scale, _scale,1);

                //시간 증가
                _t += Time.unscaledDeltaTime * 10;
                yield return null;
            }
            _color.a = dazzle_alpha;
            image_dazzle.color = _color;
            image_dazzle.rectTransform.localScale = new Vector3(dazzle_maxScale, dazzle_maxScale, 1);


            ////일정시간 유지 시키기
            yield return new WaitForSeconds(_keepTime);


            //////알파값과 스케일 값을 줄인다.
            float _timeRatio = 0; //지정시간값을 1의 비율로 지정할 값
            _t = 0.0001f;
            
            while (_t < _fadeOutTime) {

                _timeRatio = _t/ _fadeOutTime;

                if(_timeRatio > 0.33f)//일정 비율이 지난다면,
                {
                    //알파값을 줄인다.
                    _color.a = Mathf.Lerp(dazzle_alpha, 0.0f, _timeRatio);
                    image_dazzle.color = _color;
                }

                //스케일 설정
                float _scale = Mathf.Lerp(dazzle_maxScale, 0, _timeRatio);
                image_dazzle.rectTransform.localScale = new Vector3(dazzle_maxScale, _scale, 1);

                _t += Time.unscaledDeltaTime;

                yield return null;
            }

            //비활성화
            image_dazzle.enabled = false;
            dazzleCroutine = null;
        }
        #endregion

        ///Setting UI
        public void ShowSetting()
        {
            AudioManager.instance.PlayUISound("Check", 1.0f);
            
            //Window 일 경우
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
            //Window 일 경우
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






        //-------------목표

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

            if (m_ShowNoticeCroutine != null)//만약 이미 노티스가 작동 중이라면 중지한다.
            {
                StopCoroutine(m_ShowNoticeCroutine);
                m_ShowNoticeCroutine = null;
            }

            //notice_button.gameObject.SetActive(false);

            if (isSave) { note = _note; }
            
            m_ShowNoticeCroutine = ShowNoticeCroutine(_note, time);
            StartCoroutine(m_ShowNoticeCroutine);
        }



        IEnumerator ShowNoticeCroutine(string _note, float time)//일정 시간이 지난 후 투명하게 조절한다.
        {
            //생성 시 텍스터 초기값을 설정하는 것
            notice_text.enabled = true;
            notice_text.text = string.Format($"[{_note}]");
            yield return ShowNoticeFadeCroutine(true);

            yield return new WaitForSeconds(time);

            yield return ShowNoticeFadeCroutine(false);

            //notice_text.enabled = false;

            //모바일
            //if(note !=null && !GameConfig.IsPc())notice_button.gameObject.SetActive(true);

            notice_group.gameObject.SetActive(false);
            m_ShowNoticeCroutine = null;
        }

        //목표를 다시 보여줍니다.
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

            if (_bool)// 페이드 인
            {
                _alpha = 0.0f;
                targetAlpha = 1.0f;
            }
            else//페이드 아웃
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

        //기본 상태
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
        //----------end 목표


        public void ShowNoSignal()
        {
            NoSignal.Show();
        }

        [Header("마지막 기회"),Space]
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

