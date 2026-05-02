using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogElement : MonoBehaviour
{
    [SerializeField]TMP_Text txt_line;
    [SerializeField] float time = 0;
    [SerializeField] float fadeTime=1.5f;
    CanvasGroup group;
    public bool active => gameObject.activeSelf;
    DialogManager manager;

    private void Awake()
    {
        txt_line.text = string.Empty;
        group = gameObject.AddComponent<CanvasGroup>();
        Disappear();
    }

    IEnumerator enumerator = null;
    public void Set(string title, float t, float delay)
    {
        gameObject.SetActive(true);
        txt_line.text = title;
        time = t;
        group.alpha = 0;

        StopCroutine();
        enumerator = DisappearCoroutine(delay);
        StartCoroutine(enumerator);
    }

    IEnumerator DisappearCoroutine(float delay)
    {
        //딜레이 이후 활성화
        yield return new WaitForSeconds(delay);

        group.alpha = 1;
        transform.SetAsLastSibling();

        // 지문 대기
        yield return new WaitForSeconds(Mathf.Clamp(time - fadeTime, 0 , 100));

        //서서히 사라지기
        float t = 0;
        while( t < fadeTime)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(1, 0, t / fadeTime);
            yield return null;
        }

        gameObject.SetActive(false);
        enumerator = null;
    }

    //바로 사라져랏.
    public void Disappear()
    {
        StopCroutine();
        gameObject.SetActive(false);
    }

    public void SetParent(DialogManager manager) => this.manager = manager;

    public void StopCroutine()
    {
        if (enumerator != null)
        {
            StopCoroutine(enumerator);
            enumerator = null;
        }
    }
}
