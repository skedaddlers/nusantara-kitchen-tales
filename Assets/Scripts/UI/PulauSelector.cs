using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PulauSelector : MonoBehaviour
{
    [Header("Data Pulau")]
    public string namaPulau;
    [TextArea]
    public string deskripsiPulau;
    public Vector3 highlightScale = new Vector3(1.2f, 1.2f, 1f);

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPulauSelected()
    {
        // Highlight pulau
        transform.DOScale(highlightScale, 0.3f).SetEase(Ease.OutBack)
            .OnComplete(() => transform.DOScale(originalScale, 0.3f).SetDelay(0.2f));

        UIPulauSelect.Instance.ShowInfoPanel(namaPulau, deskripsiPulau);

        UIPulauSelect.Instance.ZoomToPulau(transform.position);
    }
}
