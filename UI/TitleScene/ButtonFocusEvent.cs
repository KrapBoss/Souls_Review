using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//1. 버튼에 포커싱 되면 이미지 이벤트 넣기 == FillImage
//2. 버튼 포커싱을 벗어나면 이미지가 초기화 된다.
public class ButtonFocusEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //public 
    [Header("You must Set ImageName of FillImage")]
    public Image imageFill;
    public float fillMultipleSec;

//private
    bool isFocucing = false;
    float timeRatio = 0.0f;

    private void Start()
    {
        imageFill = transform.Find("ImageFill").GetComponent<Image>();
        imageFill.fillAmount = 0;
    }

    private void Update()
    {
        if (isFocucing)
        {
            if (imageFill.fillAmount >= 1.0f) return;
            timeRatio += Time.deltaTime* fillMultipleSec;
            imageFill.fillAmount = timeRatio;
        }
        else
        {
            if (imageFill.fillAmount <= 0.0f) return;
            timeRatio -= Time.deltaTime * fillMultipleSec;
            imageFill.fillAmount = timeRatio;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isFocucing = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isFocucing = false;
        imageFill.fillAmount = timeRatio;
    }
}

