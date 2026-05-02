using UnityEngine;

public class SavingElementAnchor : MonoBehaviour
{
    public Vector2 savedAnchoredPosition;

    private void Start()
    {
        var rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = savedAnchoredPosition;
        }
    }

    public void SavePosition()
    {
        var rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            savedAnchoredPosition = rect.anchoredPosition;
        }
    }

    public void ApplySavedPosition()
    {
        var rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = savedAnchoredPosition;
        }
    }
}