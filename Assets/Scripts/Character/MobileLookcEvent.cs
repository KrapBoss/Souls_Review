using StarterAssets;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileLookcEvent : MonoBehaviour, IDragHandler, IPointerExitHandler, IEndDragHandler
{
    public float LookScale = 0.015f;

    StarterAssetsInputs inputSystem;

    Vector2 dragPosition = Vector2.zero;

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.LogWarning("Dragging");
        dragPosition = eventData.delta;

        Vector2 delta = InverseY(dragPosition);

        inputSystem.LookInput(delta * LookScale);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        inputSystem.LookInput(Vector2.zero);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inputSystem.LookInput(Vector2.zero);
    }

    public Vector2 InverseY(Vector2 v)
    {
        v.y = -v.y;
        return v;
    }


    // Start is called before the first frame update
    void Start()
    {
        inputSystem = FindObjectOfType<StarterAssetsInputs>();
    }

}
