using UnityEngine;
using UnityEngine.UI;

public class SliderButton : MonoBehaviour
{
    [Header("������ �����̴�")]
    public Slider slider;

    [Header("���� ��ư�ΰ�?")]
    public bool Right;

    [Header("������ �ƴ� ��� ���� ��")]
    public float floatValue= 0.1f;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Button) ;
    }

    public void Button()
    {
        float num = slider.value;

        if (Right)
            num += slider.wholeNumbers ? 1 : floatValue;
        else
            num -= slider.wholeNumbers ? 1 : floatValue;

        slider.value = Mathf.Clamp(num, slider.minValue, slider.maxValue);
    }
}
