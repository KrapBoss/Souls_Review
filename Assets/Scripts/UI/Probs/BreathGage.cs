using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BreathGage : MonoBehaviour
{
    [SerializeField] Image image_effect;
    [SerializeField] Image image_gage;
    [SerializeField] Image image_gagePanel;
    [SerializeField] Image image_icon;

    float t = 1.0f;

    Color color_gage;

    IEnumerator gageCroutine = null;

    private void Start()
    {
        SetColorAlpha(0);
        Color c = image_effect.color;
        c.a = 0;
        image_effect.color = c;
    }

    //�������� ������ �Ѱܹ޾� ������ �������� �����Ѵ�.
    public void SetGage(float _t , bool Can)
    {
        if (_t == t || !this.gameObject.activeSelf) return;

        //���� �����ϰ� ����� �ڷ�ƾ�� �����Ѵٸ�, �����Ѵ�.
        if (gageCroutine != null) { StopCoroutine(gageCroutine); gageCroutine = null; }
        //������ߵ� �̹����� �����ϴٸ�, �����ϰ� ���ش�.
        if(image_gage.color.a < 0.99f) { SetColorAlpha(1); }

        t = _t;

        image_icon.enabled = true;
        image_gagePanel.enabled = true;

        if (Can) image_icon.color = Color.blue;
        else image_icon.color = Color.red;

        //color_gage = new Color(0.62f, 0.72f,t);
        //image_gage.color = color_gage;
        image_gage.fillAmount = t;

        color_gage = image_effect.color;
        color_gage.a = ((1.0f-t) * 80)  / 255;
        image_effect.color = color_gage;

        //���� �������� ���� ���¶��, �����ϰ� �����.
        if(t == 1.0f) {
            gageCroutine = GageCroutine();
            StartCoroutine(gageCroutine);
        }
    }

    IEnumerator GageCroutine()
    {
        float _t = 1;
        while (_t > 0.0f)
        {
            SetColorAlpha(_t);

            _t -= Time.deltaTime;
            yield return null;
        }
        SetColorAlpha(0);
        gageCroutine = null;
    }

    Color c_croutine;
    void SetColorAlpha(float _t)
    {
        c_croutine = image_gage.color;
        c_croutine.a = _t;
        image_gage.color = c_croutine;

        c_croutine = image_gagePanel.color;
        c_croutine.a = _t;
        image_gagePanel.color = c_croutine;

        c_croutine = image_icon.color;
        c_croutine.a = _t;
        image_icon.color = c_croutine;
    }
}
