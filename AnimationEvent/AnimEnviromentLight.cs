using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 환경 조명을 변경한다.
/// </summary>
public class AnimEnviromentLight : MonoBehaviour
{
    /// <summary>
    /// 썬더 환경 변수 설정
    /// </summary>
    public void StartThunder()
    {
        StartCoroutine(StartThunderCoroutine());
    }

    IEnumerator StartThunderCoroutine()
    {
        RenderSettings.ambientLight = new Color(74/255.0f, 127/255.0f, 202/255.0f);
        yield return new WaitForSeconds(0.2f);
        RenderSettings.ambientLight = new Color(0, 27 / 255.0f, 65 / 255.0f);
        yield return new WaitForSeconds(0.2f);
        RenderSettings.ambientLight = new Color(74 / 255.0f, 127 / 255.0f, 202 / 255.0f);
        yield return new WaitForSeconds(0.2f);

        Vector3 color = new Vector3(74*1.2f / 255.0f, 127 * 1.2f / 255.0f, 202 * 1.2f / 255.0f);
        Vector3 color2 = new Vector3(DataSet.Instance.GetDefaultColor().r, DataSet.Instance.GetDefaultColor().g, DataSet.Instance.GetDefaultColor().b);

        float time = 0.0f;
        float targetTime = 10.0f;
        while (time < targetTime)
        {
            Vector3 tme_color = Vector3.Lerp(color, color2, time/ targetTime);
            RenderSettings.ambientLight = new Color(tme_color.x, tme_color.y, tme_color.z);
            yield return null;
            time += Time.deltaTime;
        }
        RenderSettings.ambientLight = new Color(color2.x, color2.y, color2.z);
    }


    // 불을 킵니다.
    public void TurnOnLight()
    {
        RenderSettings.ambientLight = DataSet.Instance.Color_Evening;
    }

    // 밤으로 변합니다.
    public void AtNight()
    {
        RenderSettings.ambientLight = DataSet.Instance.Color_Default * 0.5f;
    }
}
