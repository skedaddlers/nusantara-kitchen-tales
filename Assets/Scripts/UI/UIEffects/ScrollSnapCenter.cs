using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollSnapCenter : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform viewport;

    private bool isSnapping = false;
    [SerializeField]
    private float snapCooldown = 0.3f;
    [SerializeField]
    private float velocityThreshold = 100f;

    void Update()
    {
        if (isSnapping || scrollRect.velocity.magnitude > velocityThreshold) return;

        // Snap setelah swipe selesai
        isSnapping = true;
        Invoke(nameof(SnapToClosest), snapCooldown);
    }

    void SnapToClosest()
    {
        float closestDist = float.MaxValue;
        RectTransform closestCard = null;

        foreach (Transform child in content)
        {
            RectTransform card = child.GetComponent<RectTransform>();
            float distance = Mathf.Abs(GetWorldX(card) - GetWorldX(viewport));
            if (distance < closestDist)
            {
                closestDist = distance;
                closestCard = card;
            }
        }

        if (closestCard != null)
        {
            // Geser content agar closestCard ke tengah viewport
            float targetX = content.anchoredPosition.x +
                            (viewport.rect.width / 2 - (closestCard.localPosition.x + closestCard.rect.width / 2));

            content.DOAnchorPosX(targetX, 0.4f).SetEase(Ease.OutCubic);
        }

        isSnapping = false;
    }

    float GetWorldX(RectTransform rt)
    {
        return rt.TransformPoint(rt.rect.center).x;
    }
}
