using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public Image img_fade;

    public void FadeStart(bool fadein, float time, Color c)
    {
        DontDestroyOnLoad(this.gameObject);
        StartCoroutine(FadeCroutine(fadein, time, c));
    }
    IEnumerator FadeCroutine(bool fadein, float time, Color c)
    {
        //페이드 시작을 알림
        b_faded = false;

        float from, to;
        if (fadein) { from = 0.0f; to = 1.0f; } else { from = 1.0f; to = 0.0f; }

        c.a = from;
        img_fade.color = c;

        float currTime = 0.0f;
        float ratio = 0.0f;

        while (ratio < 1.0f)
        {
            currTime += Time.unscaledDeltaTime;
            ratio = currTime / time;

            c.a = Mathf.Lerp(from, to, ratio);
            img_fade.color = c;

            yield return null;
        }

        //FadeOut일 경우 게임오브젝트를 제거한다.
        if (!fadein)
        {
            Destroy(this.gameObject);
        }

        //페이드 완료됨
        b_faded = true;

        //혹시 모를 경우를 대비
        yield return new WaitForSeconds(5.0f);
        Destroy(this.gameObject);
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }


    //페이드 동작 완료 여부
    public static bool b_faded;
    //이전 페이드 오브젝트를 담는다.
    static GameObject go_FadeIn = null;
    //Fade 동작 실행을 위한 전역 함수
    public static void FadeSetting(bool fadein, float time, Color c)
    {
        //페이드 오브젝트 가져오기
        GameObject goFade = Resources.Load<GameObject>("Prefabs/CanvasFade");

        //페이드 객체가 있을 경우 제거합니다.
        if (go_FadeIn != null && goFade != null)
        {
            Destroy(go_FadeIn);
        }

        go_FadeIn = Instantiate(goFade);
        //객체 생성
        Fade fade = go_FadeIn.GetComponent<Fade>();

        //실행
        fade.FadeStart(fadein, time, c);

        goFade = null;
        Resources.UnloadUnusedAssets();
    }
}
