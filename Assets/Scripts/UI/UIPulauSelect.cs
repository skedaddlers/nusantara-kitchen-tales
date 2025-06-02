using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIPulauSelect : MonoBehaviour
{
    public RectTransform mapContainer;


    [Header("Data Pulau")]
    public CanvasGroup infoPanel;
    public TextMeshProUGUI namaPulauText;
    public TextMeshProUGUI deskripsiText;
    public Button tombolMasuk;

    private void Start()
    {
        infoPanel.alpha = 0;
        infoPanel.interactable = false;
        infoPanel.blocksRaycasts = false;
    }

    public void ShowInfoPanel(string namaPulau, string deskripsiPulau, PulauDataSO pulauData)
    {
        namaPulauText.text = namaPulau;
        deskripsiText.text = deskripsiPulau;

        infoPanel.DOFade(1f, 0.3f);
        infoPanel.interactable = true;
        infoPanel.blocksRaycasts = true;

        // tombolMasuk.onClick.RemoveAllListeners();
        tombolMasuk.onClick.AddListener(() => MasukPulau(namaPulau, pulauData));
    }

    public void ZoomToPulau(Vector3 targetPosition)
    {
        Vector3 canvasCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
    
        Vector3 offset = canvasCenter - targetPosition;

        mapContainer.DOAnchorPos(mapContainer.anchoredPosition + (Vector2)offset, 0.5f).SetEase(Ease.OutQuad);
        mapContainer.DOScale(1.5f, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            Debug.Log("Zoom selesai, kembali ke posisi awal.");
        });

    }

    public void CloseInfoPanel()
    {
        Debug.Log("Tutup info panel");
        infoPanel.DOFade(0f, 0.3f).OnComplete(() =>
        {
            infoPanel.interactable = false;
            infoPanel.blocksRaycasts = false;
        });

        ZoomOutPulau();

        // Reset tombolMasuk listener
        tombolMasuk.onClick.RemoveAllListeners();
    }

    private void ZoomOutPulau()
    {
        // Kembali ke posisi awal
        mapContainer.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutQuad);
        mapContainer.DOScale(1f, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            Debug.Log("Zoom out selesai, kembali ke posisi awal.");
        });
    }

    void MasukPulau(string namaPulau, PulauDataSO pulauData)
    {
        Debug.Log("Masuk ke pulau: " + namaPulau);
        GameData.PulauDipilih = pulauData;
        SceneLoader.LoadScene("StageSelect");
    }
}