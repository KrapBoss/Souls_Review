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
        //���̵� ������ �˸�
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

        //FadeOut�� ��� ���ӿ�����Ʈ�� �����Ѵ�.
        if (!fadein)
        {
            Destroy(this.gameObject);
        }

        //���̵� �Ϸ��
        b_faded = true;

        //Ȥ�� �� ��츦 ���
        yield return new WaitForSeconds(5.0f);
        Destroy(this.gameObject);
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }


    //���̵� ���� �Ϸ� ����
    public static bool b_faded;
    //���� ���̵� ������Ʈ�� ��´�.
    static GameObject go_FadeIn = null;
    //Fade ���� ������ ���� ���� �Լ�
    public static void FadeSetting(bool fadein, float time, Color c)
    {
        //���̵� ������Ʈ ��������
        GameObject goFade = Resources.Load<GameObject>("Prefabs/CanvasFade");

        //���̵� ��ü�� ���� ��� �����մϴ�.
        if (go_FadeIn != null && goFade != null)
        {
            Destroy(go_FadeIn);
        }

        go_FadeIn = Instantiate(goFade);
        //��ü ����
        Fade fade = go_FadeIn.GetComponent<Fade>();

        //����
        fade.FadeStart(fadein, time, c);

        goFade = null;
        Resources.UnloadUnusedAssets();
    }
}
