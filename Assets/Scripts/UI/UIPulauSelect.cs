using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIPulauSelect : MonoBehaviour
{
    public static UIPulauSelect Instance { get; private set; }

    [Header("Data Pulau")]
    public CanvasGroup infoPanel;
    public TextMeshProUGUI namaPulauText;
    public TextMeshProUGUI deskripsiText;
    public Button tombolMasuk;

    private void Start()
    {
        // Singleton pattern to ensure only one instance of UIPulauSelect exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        infoPanel.alpha = 0;
        infoPanel.interactable = false;
        infoPanel.blocksRaycasts = false;
    }

    public void ShowInfoPanel(string namaPulau, string deskripsiPulau)
    {
        namaPulauText.text = namaPulau;
        deskripsiText.text = deskripsiPulau;

        infoPanel.DOFade(1f, 0.3f);
        infoPanel.interactable = true;
        infoPanel.blocksRaycasts = true;

        tombolMasuk.onClick.RemoveAllListeners();
        tombolMasuk.onClick.AddListener(() => MasukPulau(namaPulau));
    }

    public void CloseInfoPanel()
    {
        Debug.Log("Tutup info panel");
        infoPanel.DOFade(0f, 0.3f).OnComplete(() =>
        {
            infoPanel.interactable = false;
            infoPanel.blocksRaycasts = false;
        });
    }

    void MasukPulau(string namaPulau)
    {
        Debug.Log("Masuk ke pulau: " + namaPulau);
        // Ganti scene sesuai pulau:
        // SceneLoader.LoadScene(SceneLoader.Scene.Jawa);
    }
}