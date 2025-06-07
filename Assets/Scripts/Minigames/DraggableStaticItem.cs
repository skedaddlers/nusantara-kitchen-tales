using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableStaticItem : DraggableItem
{
    [Header("Static Container")]
    public RectTransform staticContainer; // Gelas, mangkuk, dll
    public RectTransform movingContent;   // Isi yang bisa dipindah
    public Sprite contentSprite; // Sprite untuk movingContent
    public Image contentImage;   // Komponen Image untuk movingContent

    private Vector2 containerStartPos;

    protected override void Awake()
    {
        base.Awake();

        contentImage.sprite = contentSprite;
        // dont show contentImage until movingContent is dragged
        contentImage.enabled = false;

        if (staticContainer == null || movingContent == null)
        {
            Debug.LogError("DraggableStaticItem: Assign both staticContainer and movingContent!");
        }

        containerStartPos = staticContainer.anchoredPosition;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        containerStartPos = staticContainer.anchoredPosition;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (movingContent != null)
        {
            movingContent.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        if (!droppedCorrectly && movingContent != null)
        {
            staticContainer.anchoredPosition = containerStartPos; // return to start position     
            movingContent.anchoredPosition = Vector2.zero; // reset movingContent position       
        }
        if (droppedCorrectly && movingContent != null)
        {
            // return the container to the staticContainer position
            staticContainer.anchoredPosition = containerStartPos;

            contentImage.enabled = true; // Show the content image when dropped correctly

        }
    }
}
