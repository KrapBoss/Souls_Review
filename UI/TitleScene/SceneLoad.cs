using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Random = UnityEngine.Random;


public class SceneLoad : MonoBehaviour
{

    public static SceneLoad Instance { get; set; }

    [SerializeField] CanvasGroup c_group;

    [SerializeField] Image image_progress;

    [SerializeField] TMP_Text loadingTip;

    string CurrentSceneName ="TitleScene";

    public string name_Title = "TitleScene";
    public string name_Main = "Main";

    //로딩이 끝나고 나서의 실행 함수를 지정합니다.
    public Action action_endLoad;

    //씬을 불러오고 있는 경우
    public bool Loading = false;

    private void Awake()
    {

        if (Instance == null) { Instance = this; DontDestroyOnLoad(this.gameObject); }
        else { Destroy(this.gameObject); }

        gameObject.SetActive(false);
    }

    //씬을 로딩합니다.
    // ReSharper disable Unity.PerformanceAnalysis
    public void Load(string _name, bool tutorial= false)
    {
        gameObject.SetActive(true);

        CurrentSceneName = _name;

        StartCoroutine(LoadCroutine(tutorial));
    }

    public void SetLoadEndAction(Action _action)
    {
        action_endLoad = _action;
    }

    IEnumerator LoadCroutine(bool tutorial = false)
    {
        Loading = true;

        //텍스트 표시
        loadingTip.text = LocalLanguageSetting.Instance.GetLocalText("Tip", $"Loading{Random.Range(1, 3)}");

        //step 1 fadeinScreen
        yield return FadeInCroutine();

        Debug.Log("씬을 로드 합니다 : " + CurrentSceneName);

        AsyncOperation _operation = SceneManager.LoadSceneAsync(CurrentSceneName);
        _operation.allowSceneActivation = false;

        //step 2 view progressBar
        float _progress = 0.0f;
        while (_progress < 0.89f)
        {
            if(_progress > _operation.progress) { continue; }

            _progress += Time.unscaledDeltaTime * 0.333f;

            image_progress.fillAmount = _progress;

            yield return null;
        }

        //0.9까지 로드가 완료되면 씬을 로드시킨다.
        _operation.allowSceneActivation = true;
        //작업이 끝날때까지 대기 시킨다.
        while (!_operation.isDone)
        {
            Debug.Log("맵을 불러오고 있습니다.");
            yield return null;
        }

        //2초 지연한다.
        float time =0;
        while (time <1.0f)
        {
            time += Time.unscaledDeltaTime * 0.5f;
            image_progress.fillAmount = Mathf.Lerp(0.9f, 1.0f, time);
        }

        //메인일 경우 튜토리얼 구분을 위한 동작
        if (CurrentSceneName.Equals("Main"))
        {
            yield return new WaitUntil(() => GameManager.Instance);
            GameManager.Instance.GameStart(tutorial);
        }


        //화면을 보여준다.
        yield return FadeOutCroutine();

        //씬 하면서 지정된 동작을 수행합니다.
        if (action_endLoad != null) { action_endLoad(); action_endLoad = null; }

        Debug.Log("씬을 로드에 성공했습니다. : " + CurrentSceneName);

        //종료를 알림
        Loading = false;
        Time.timeScale = 1.0f;


        //비활성화
        gameObject.SetActive(false);
    }

    //화면을 가린다.
    IEnumerator FadeInCroutine()
    {
        float t = 0.0f;

        while (t < 1.0f)
        {
            c_group.alpha = t;

            t += Time.unscaledDeltaTime * 0.5f;

            yield return null;
        }
        yield return null;

        c_group.alpha = 1;

        Debug.Log("화면 Fadein 되었습니다.");
    }

    //화면을 보여준다.
    IEnumerator FadeOutCroutine()
    {
        float t = 1.0f;

        while (t > 0.0f)
        {
            c_group.alpha = t;

            t -= Time.unscaledDeltaTime * 0.5f;

            yield return null;
        }

        c_group.alpha = 0;


        Debug.Log("화면 Fadeout 되었습니다.");
    }
}
