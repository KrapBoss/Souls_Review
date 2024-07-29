using CustomUI;
using StarterAssets;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

/// <summary>
/// 배선 박스 UI에 대한 스크립트
/// </summary>
public class WireBox : MonoBehaviour
{
    //배전 활성 여부
    //public static bool Active;
    //배전 성공 개수
    public static short SuccessCount =0;

    //게이지 판정 구간
    public Image img_Boundary;
    //움직이는 점
    public Image img_dot;
    //게이지들을 담고 있는 부모
    public RectTransform rect_GageParent;
    //점이 회전하는 속도
    public float dotSpeed = 3.0f;
    private float _applyDotSpeed;

    //사운드
    public AudioClip clip_CheckIn;
    public AudioClip clip_CheckOut;
    public AudioSource source;

    //버튼
    public Button btn;

    public float offsetBoundary = 20.0f;

    public GameObject Icon_Text;

    //게이지들을 회전 시킬 반경
    private float _randomAngle;
    //게이지 판정 범위
    private float _randomRange;
    //움직이는 점의 각도
    private float _dotAngle;
    //성공 횟수
    private int _successCount = 0;
    //실패 횟수
    private int _failedCount = 0;

    private float[] rangeBoundary = new float[] { 0.18f, 0.12f, 0.07f };

    StarterAssetsInputs _inputs;

    //배전함을 성공했을 경우 배전함 이벤트를 실행함.
    Action<bool> _successAction;

    private void OnEnable()
    {
        //Active = true;

        //모바일의 경우 컨트롤 패널 비활성화
        if (!GameConfig.IsPc())
        {
            UI.mobileControllerUI.SetActive(false);

            //버튼 이벤트 활성화
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ButtonAction);

            Icon_Text.SetActive(false);
        }
        else
        {
            //마우스 보여주기
            //GameManager.Instance.CursorShow();

            //버튼 비활성화
            btn.gameObject.SetActive(false);

            Icon_Text.SetActive(true);
        }

        //초기화
        _successCount = 0;
        _failedCount = 0;
        dotSpeed = DataSet.Instance.GameDifficulty.WireBoxDotSpeed;
       _applyDotSpeed = dotSpeed;
        RandomBoundary();


        if (_inputs == null)
            _inputs = FindObjectOfType<StarterAssetsInputs>();
    }

    private void OnDisable()
    {
        //Active = false;
        //마우스 숨기기
        GameManager.Instance.CursorHide();

        //활성화
        if (!GameConfig.IsPc())
        {
            UI.mobileControllerUI.SetActive(true);
        }
    }

    private void Update()
    {
        DotRotate();

        // 판정
        if (_inputs.Grab)
        {
            ButtonAction();
            _inputs.Grab = false;
        }
    }

    public void ButtonAction()
    {
        if (CheckInRange())
        {
            Debug.Log("전선함 판정 성공");
            CheckInSuccess();
        }
        else
        {
            Debug.Log("전선함 판정 실패");
            CheckInFailed();
        }
    }


    //판정 구역을 랜덤으로 설정합니다.
    void RandomBoundary()
    {
        _randomAngle = Random.Range(80.0f, 360.0f);
        _randomRange = rangeBoundary[_successCount];
        _dotAngle = Random.Range(0.0f, 360.0f);

        img_Boundary.fillAmount = _randomRange;
        rect_GageParent.rotation = Quaternion.Euler(0, 0, _randomAngle);

        float x = ((rect_GageParent.rect.width * .3f) + offsetBoundary) * Mathf.Cos(Mathf.Deg2Rad * _dotAngle) + Screen.width * 0.5f;
        float y = ((rect_GageParent.rect.height * .3f) + offsetBoundary) * Mathf.Sin(Mathf.Deg2Rad * _dotAngle) + Screen.height * 0.5f;
        img_dot.rectTransform.position = new Vector3(x, y, 0);
    }

    //배전함 범위 안에 점이 존재하는 경우를 판단.
    bool CheckInRange()
    {
        float s =_randomAngle ;
        float e = _randomAngle - 360.0f * _randomRange;

        float angle = (_dotAngle +  90.0f) % 360.0f;
        if (s >= angle && e <= angle)
            return true;
        else
           return false;
    }

    void DotRotate()
    {
        _dotAngle = (_dotAngle + Time.unscaledDeltaTime * _applyDotSpeed) % 360.0f;
        float x = ((rect_GageParent.rect.width * 0.35f) + offsetBoundary) * Mathf.Cos(Mathf.Deg2Rad*_dotAngle) + Screen.width * 0.5f;
        float y = ((rect_GageParent.rect.height * 0.35f) + offsetBoundary) * Mathf.Sin(Mathf.Deg2Rad * _dotAngle) + Screen.height * 0.5f;
        img_dot.rectTransform.position = new Vector3(x, y, 0);
    }

    void CheckInSuccess()
    {
        _successCount++;
        //총 3번 성공
        if(_successCount > 2)
        {
            Debug.Log("배전 수리가 완료되었습니다");

            //배전 성공 횟수 누적
            SuccessCount++;

            //배전함 지정 이벤트 실행
            if (_successAction != null) _successAction(true);

            //게임 정지 종료
            GameManager.Instance.FrozenOffGame();
        }
        else
        {
            //스케일 효과
            StartCoroutine(GageScaleChangeCroutine(_successCount));

            //성공 쾅 사운드`
            source.PlayOneShot(clip_CheckIn);

            //다시 배치
            RandomBoundary();

            //사운드 범위 넘기기
            EventManager.instance.EventSound(PlayerEvent.instance.transform, 99.9f);

            //속도 증가
            _applyDotSpeed *= 1.5f;
        }
    }

    //Time에 제한되지 않는 스케일 조절 이펙트
    IEnumerator GageScaleChangeCroutine(int repeat)
    {
        for(int i=0;i<repeat; i++)
        {
            float timer = 0.0f;

            rect_GageParent.localScale = new Vector3(0.8f, 0.8f);

            while (timer < 0.03f)
            {
                timer  += Time.unscaledDeltaTime;
                yield return null;
            }
            timer = 0;

            rect_GageParent.localScale = new Vector3(1.2f, 1.2f); 

            while (timer < 0.03f)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            timer = 0;

            rect_GageParent.localScale = new Vector3(0.9f, 0.9f);

            while (timer < 0.03f)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            timer = 0;

            rect_GageParent.localScale = new Vector3(1.0f, 1.0f);

            while (timer < 0.03f)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }

    //배전함을 실패했을 경우
    void CheckInFailed()
    {
        _failedCount++;

        Fade.FadeSetting(false, 0.3f, new Color(0.7f, 0.0f ,0.0f));

        // 치직  실패 사운드
        source.PlayOneShot(clip_CheckOut);

        //3번 실패했는가?
        if (_failedCount > 2)
        {
            EventManager.instance.EventSound(PlayerEvent.instance.transform, 50.0f);

            //게임 정지 풀지
            GameManager.Instance.FrozenOffGame();

            if (_successAction != null) _successAction(false);

            //화면 붉게
            Fade.FadeSetting(true, 0.5f, new Color(0.3f, .3f, 1.0f));

            //삐이 소리
            AudioManager.instance.PlayEffectiveSound("Tinnitus", 1.0f);

            //기절
            PlayerEvent.instance.PlayerFaint(ShowText);
        }
        else
        {
            RandomBoundary();
            StartCoroutine(GageScaleChangeCroutine(_failedCount *3));
        }
    }

    public void ShowText()
    {
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", $"WireBox{Random.Range(0, 3)}"), 3.0f);
        UI.staticUI.ShowLine();
    }

    //현재 창을 닫습니다.
    public void Close()
    {
        Debug.Log("전기함을 닫습니다.");
        this.gameObject.SetActive(false);
    }

    public void SetEvent(Action<bool> action)
    {
        _successAction = action;
    }
}
