using Cinemachine;
using CustomUI;
using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;


//죽었을 때 애니메이션
public class DeadScene : MonoBehaviour
{
    [Header("Public")]
    public CinemachineVirtualCamera VirtualCamera;
    //데드씬의 끝을 알림
    public bool endDeadScene;
    public AudioSource source;

    [Header("White")]
    public Transform Head;
    public float paintHeadScale = 8.3f;
    public float paintTime = 2.0f;

    public GameObject Part1;
    public GameObject Part2;

    public Transform start;
    public Transform end;


    [Space,Header("Black")]
    public GameObject Part1_black;
    public Transform position_Start1;
    public Transform position_End1;

    public GameObject Part2_black;
    public Transform position_Start2;
    public Transform position_End2;
    [Header("마지막 배열을 Default")]
    public GameObject[] go_Moment;
    public float random_Black = 7.0f;

    //발자국
    public AudioClip clip_Black1;
    public AudioClip clip_Black2; // 소리지르기


    private void Start()
    {
        Init();
    }

    #region white
    //기절
    public void PaintWhite(Action EndAction)
    {
        StartCoroutine(PaintCroutine(EndAction));
    }

    WaitForSeconds waitPaint = new WaitForSeconds(0.05f);
    public IEnumerator PaintCroutine(Action action)
    {
        endDeadScene = false;

        if (!GameConfig.IsPc())
        {
            UI.mobileControllerUI.SetActive(false);
        }

        Init();

        //init
        Part1.SetActive(true);
        Part2.SetActive(true);

        Part1.transform.position = start.position;
        Head.localScale = new Vector3(1f, 1f, 1f);

        Part2.SetActive(false);

        //화면 흔들기
        float timeOut = paintTime;
        while (timeOut > 0.0f)
        {
            Part1.transform.position = Vector3.Lerp(start.position, end.position, 1 - (timeOut / paintTime));

            VirtualCamera.m_Lens.Dutch += Random.Range(-0.5f, 0.5f);
            yield return waitPaint;
            timeOut -= 0.05f;
        }

        //2번째 효과 활성
        Part1.SetActive(false);
        Part2.SetActive(true);

        timeOut = 1.5f;
        while (timeOut > 0.0f)
        {
            Part1.transform.position = Vector3.Lerp(start.position, end.position, 1 - (timeOut / paintTime));

            VirtualCamera.m_Lens.Dutch += Random.Range(-0.5f, 0.5f);
            yield return waitPaint;
            timeOut -= 0.05f;
        }


        AudioManager.instance.PlayEffectiveSound("GhostScream", 1.7f);

        //잡아먹기
        float scale = Head.localScale.x;
        while (Head.localScale.x < paintHeadScale)
        {
            scale += paintHeadScale * Time.unscaledDeltaTime * 4;
            Head.localScale = new Vector3(1, 1, 1) * scale;
            yield return null;
        }

        Fade.FadeSetting(true, 0.5f, Color.black);

        //모바일 컨트롤바 실행
        if (!GameConfig.IsPc())
        {
            UI.mobileControllerUI.SetActive(true);
        }

        Part1.SetActive(false);
        Part2.SetActive(false);

        endDeadScene = true;
        if (action != null) action();
    }
    #endregion

    #region black
    //기절
    public void PaintBlack(Action EndAction)
    {
        StartCoroutine(PaintCroutineBlack(EndAction));
    }
    IEnumerator PaintCroutineBlack(Action action)
    {
        endDeadScene = false;

        if (!GameConfig.IsPc())
        {
            UI.mobileControllerUI.SetActive(false);
        }

        Init();

        //초기화
        Part1_black.SetActive(true);
        Part2_black.SetActive(false);

        source.clip = clip_Black1;
        source.Play();
        source.pitch = 3.0f;
        source.loop = true;

        //위치 초기화
        Part1_black.transform.localPosition = Vector3.right * random_Black*2.0f;

        //첫번째. 블랙이가 멀리에서 발자국 소리를 내며 다가온다.
        float t = 0;
        int desub = 0;
        while (t < 1.0f)
        {
            t += Time.unscaledDeltaTime * 0.33f;

            int a = Mathf.FloorToInt(t / 0.2f);
            if (a > desub)
            {
                desub = a;
                position_Start1.localPosition = new Vector3(position_Start1.localPosition.x, Random.Range(-random_Black, random_Black), Random.Range(-random_Black, random_Black));
            }

            Part1_black.transform.localPosition = Vector3.Lerp(position_Start1.localPosition, position_End1.localPosition, t);
            source.volume = Mathf.Lerp(DataSet.Instance.SettingValue.Volume*.3f, DataSet.Instance.SettingValue.Volume*1.3f, t);
            yield return null;
        }

        source.Stop();

        //두번째--------- INit
        Part1_black.SetActive(false);
        Part2_black.SetActive(true);
        Part2_black.transform.localScale = Vector3.zero;
        Part2_black.transform.position = position_Start2.position;

        //오브젝트 안보이기
        foreach (var go in go_Moment) go.SetActive(false);
        go_Moment[go_Moment.Length - 1].SetActive(true);

        //대기시간
        t = 0;
        while (t < 1.0f)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }


        //소리지르는 사운드 설정
        source.clip = clip_Black2;
        source.loop = false;
        source.pitch = 1.2f;
        source.Play();

        //대기시간
        //t = 0;
        //while (t < 0.5f)
        //{
        //    t += Time.unscaledDeltaTime;
        //    yield return null;
        //}

        //괴물의 모습이 바뀌는 것을 표현
        float momentTime = 0.5f;
        t = 0;
        // 커지기
        while (t < momentTime)
        {
            t+= Time.unscaledDeltaTime;

            float ratio = (t / momentTime) * 1.3f;

            Part2_black.transform.localScale = new Vector3(ratio, ratio, ratio);
            source.volume = Mathf.Lerp(0, DataSet.Instance.SettingValue.Volume * 1.7f, t);

            yield return null;
        }

        //뛰어들기
        momentTime = 0.1f;
        int index = 0;
        while (index < go_Moment.Length)
        {
            //위치값 초기화 및 사운드와, 비율 설정
            source.pitch = 0.8f - index * 0.3f;
            source.volume = DataSet.Instance.SettingValue.Volume;
            t = (float)index/(go_Moment.Length-1);
            Part2_black.transform.localPosition = Vector3.Lerp(position_Start2.localPosition, position_End2.localPosition, t);

            go_Moment[index].SetActive(true);

            //대기시간
            float time = 0;
            while (time < momentTime)
            {
                time += Time.unscaledDeltaTime;
                yield return null;
            }

            source.volume = 0.0f;
            go_Moment[index].SetActive(false);

            //대기시간
            time = 0;
            while (time < momentTime*0.5f)
            {
                time += Time.unscaledDeltaTime;
                yield return null;
            }

            index++;
        }

        Fade.FadeSetting(true, 0.5f, Color.black);

        //모바일 컨트롤바 실행
        if (!GameConfig.IsPc())
        {
            UI.mobileControllerUI.SetActive(true);
        }
        endDeadScene = true;
        if (action != null) action();
    }

    #endregion


    void Init()
    {
        Part1.SetActive(false); Part2.SetActive(false);

        Part1_black.SetActive(false); Part2_black.SetActive(false);
    }
}