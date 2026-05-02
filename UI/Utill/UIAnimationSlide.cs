using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationSlide : UIAnimationParent
{
    public RectTransform start;
    public RectTransform end;
    public float time = 0.5f;

    RectTransform m_rect;

    public override void Hide()
    {
        Find();
        StopAllCoroutines();
        m_rect.anchoredPosition = start.anchoredPosition;
    }

    public override void Show()
    {
        Find();

        m_rect.anchoredPosition = start.anchoredPosition;

        StartCoroutine(ShowCor());
    }

    void Find()
    {
        if(m_rect == null)m_rect = GetComponent<RectTransform>();
    }

    public IEnumerator ShowCor()
    {
        float t = 0;
        while(t < time)
        {
            t += Time.deltaTime;

            m_rect.anchoredPosition = Vector3.Lerp(m_rect.anchoredPosition,end.anchoredPosition, t/time);


            yield return null;
        }
        m_rect.anchoredPosition = end.anchoredPosition;
    }
}
