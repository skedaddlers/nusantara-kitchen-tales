using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ButtonPulauManager : MonoBehaviour
{


    [Header("UI Pulau")]
    public RectTransform panelPulauContainer;
    public PulauDataSO pulauData;
    public Button jawaButton;
    public Button sumatraButton;
    public Button kalimantanButton;
    public Button sulawesiButton;
    public Button papuaButton;
    public GameObject pulauInfoPanel;
    public TextMeshProUGUI namaPulauText;
    public TextMeshProUGUI deskripsiText;
    public Button tombolSilang;
    public Button tombolMasuk;
    public Button homeButton;

    private Vector3 originalPanelPos;
    private Vector3 originalPanelScale;
    public float zoomScale = 2f;

    private void Start()
    {

        originalPanelPos = panelPulauContainer.localPosition;
        originalPanelScale = panelPulauContainer.localScale;

        jawaButton.onClick.AddListener(() => OnPulauButtonClicked(jawaButton));
        sumatraButton.onClick.AddListener(() => OnPulauButtonClicked(sumatraButton));
        kalimantanButton.onClick.AddListener(() => OnPulauButtonClicked(kalimantanButton));
        sulawesiButton.onClick.AddListener(() => OnPulauButtonClicked(sulawesiButton));
        papuaButton.onClick.AddListener(() => OnPulauButtonClicked(papuaButton));

        tombolSilang.onClick.AddListener(() => EnableAllButtons());
        tombolMasuk.onClick.AddListener(() =>
        {
            GameData.PulauDipilih = pulauData;
            pulauInfoPanel.SetActive(false);
            SceneLoader.LoadScene("StageSelect");
        });
        homeButton.onClick.AddListener(() =>
        {
            GameData.ResetData();
            SceneLoader.LoadScene("MainMenu");
        });
        // set panel inactive initially
        pulauInfoPanel.SetActive(false);
    }

    private void EnableAllButtons()
    {
        jawaButton.interactable = true;
        sumatraButton.interactable = true;
        kalimantanButton.interactable = true;
        sulawesiButton.interactable = true;
        papuaButton.interactable = true;

        pulauInfoPanel.SetActive(false);

        // Reset zoomed panel
        panelPulauContainer.DOKill();
        panelPulauContainer.DOScale(originalPanelScale, 0.5f).SetEase(Ease.OutCubic);
        panelPulauContainer.DOLocalMove(originalPanelPos, 0.5f).SetEase(Ease.OutCubic);
    }



    private void OnPulauButtonClicked(Button btn)
    {
        SetOtherButtonsInactive(btn);
        pulauData = btn.GetComponent<Pulau>().pulauDataSO;

        // Offset to leave space for info panel
        float xOffset = panelPulauContainer.rect.width * -0.25f;


        Vector3 btnLocalPos = (Vector2)panelPulauContainer.InverseTransformPoint(btn.transform.position);
        Vector3 targetPos = (-btnLocalPos * zoomScale) + new Vector3(xOffset, 0f, 0f);

        panelPulauContainer.DOKill();
        panelPulauContainer.DOScale(zoomScale, 0.5f).SetEase(Ease.OutCubic);
        panelPulauContainer.DOLocalMove(targetPos, 0.5f).SetEase(Ease.OutCubic);

        DOVirtual.DelayedCall(0.4f, () =>
        {
            pulauInfoPanel.SetActive(true);
            namaPulauText.text = pulauData.namaPulau;
            deskripsiText.text = pulauData.deskripsiPulau;

            pulauInfoPanel.transform.localScale = Vector3.zero;
            pulauInfoPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        });
    }





    private void SetOtherButtonsInactive(Button activeButton)
    {
        Button[] buttons = { jawaButton, sumatraButton, kalimantanButton, sulawesiButton, papuaButton };
        foreach (Button btn in buttons)
        {
            if (btn != activeButton)
            {
                btn.interactable = false;
            }
            else
            {
                btn.interactable = true;
            }
        }
    }


}