using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class NoSignal : MonoBehaviour
{
    CanvasGroup _canvasGroup;
    AudioSource _source;
    public void Show()
    {
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
        _source = GetComponent<AudioSource>();

        gameObject.SetActive(true);
        _source.volume = DataSet.Instance.SettingValue.Volume * 0.7f;
        _source.Play();
        StartCoroutine(ShowCroutine());
    }

    IEnumerator ShowCroutine() 
    {
        for(int i =0;i< 5; i++)
        {
            float t = 1.0f;
            while (t > 0.0f)
            {
                yield return null;
                t -= Time.deltaTime * 2.0f;
                _canvasGroup.alpha = t;
            }
            t = 0.0f;
            while (t < 1.0f)
            {
                yield return null;
                t += Time.deltaTime*2.0f;
                _canvasGroup.alpha = t;
            }
        }

        //Å¸ÀÌÆ² ¾ÀÀ» ºÎ¸¨´Ï´Ù.
        SceneLoader.LoadLoaderScene("TitleScene");
    }
}
