using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected Canvas canvas;
    protected RectTransform rectTransform;
    protected CanvasGroup canvasGroup;
    protected Vector2 startPos;


    public bool droppedCorrectly = false;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        startPos = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (!droppedCorrectly)
        {
            rectTransform.anchoredPosition = startPos; // balikin ke posisi awal
        }
        else
        {
            rectTransform.localScale = new Vector3(0.8f, 0.8f, 1f); // kecilin item
            DisableDraggable(); // disable item jika sudah dropped correctly
        }
    }

    public void DisableDraggable()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
}
