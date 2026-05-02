using System;
using Unity.VisualScripting;
using UnityEngine;

public class SizeFixer : MonoBehaviour
{
    [Header("∫Ò¿≤")]
    public float heightRatio;
    public float widthRatio;

    public Canvas canvas;

    private void Awake()
    {
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();

        float screenX = rectTransform.rect.width;
        float screenY = rectTransform.rect.height;

        GetComponent<RectTransform>().sizeDelta = new Vector2(screenX * widthRatio, screenY * heightRatio);
    }
}
