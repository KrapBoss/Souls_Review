using UnityEngine;
using UnityEngine.UI;

public class SliderButton : MonoBehaviour
{
    [Header("적용할 슬라이더")]
    public Slider slider;

    [Header("증가 버튼인가?")]
    public bool Right;

    [Header("정수가 아닌 경우 증감 값")]
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
