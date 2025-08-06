using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StageSelect : MonoBehaviour
{
    public PulauDatabase database;
    public Transform cardContainer;
    public GameObject stageCardPrefab;
    public Image backgroundImage;
    public Button homeButton;
    public GameObject stagePanel;
    public GameObject notAvailablePanel;
    public Button notAvailableButton;
    public Button startButton;
    public Button closePanelButton;
    public Button quiz;
    public Button test;
    public Button practice;
    public GameObject comingSoonPanel;
    public Button comingSoonButton;
    public Image titleImage;
    public Image resepImage;

    [Header("Panel Progres")]
    public Image[] normalModeStars; // Masukkan 3 GameObject Image bintang untuk mode Normal
    public Image[] skillTestStars; // Masukkan 3 GameObject Image bintang untuk Uji Kemampuan

    // list of all stage cards
    private StageCardUI[] stageCards;

    private void Start()
    {
        // add button listeners
        startButton.onClick.AddListener(() =>
        {
            // Animasi efek klik
            transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 10, 1).OnComplete(() =>
            {
                GameData.GameType = GameType.Normal; 
                // Simpan nama stage, dan Load scene gameplay, tapi hanya pecel saja yang sudah ada
                if (GameData.ResepDipilih.namaResep == "Nasi Pecel")
                {
                    AudioManager.Instance.PlayMusic("gameplay");
                    SceneLoader.LoadScene("Story");
                }
                else
                {
                    notAvailablePanel.SetActive(true);
                    notAvailableButton.onClick.AddListener(() =>
                    {
                        // Close not available panel
                        notAvailablePanel.SetActive(false);
                        // Reset selected recipe
                        GameData.ResepDipilih = null;
                        // Hide stage panel
                    });
                    stagePanel.SetActive(false);
                    // animate not available panel
                    notAvailablePanel.transform.localScale = Vector3.zero;
                    notAvailablePanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

                }
            });
        });

        quiz.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene("Quiz");
            // Debug.Log("Quiz button clicked, but not implemented yet.");
            // comingSoonPanel.SetActive(true);
            // comingSoonButton.onClick.AddListener(() =>
            // {
            //     comingSoonPanel.SetActive(false);
            // });
        });

        test.onClick.AddListener(() =>
        {
            // Debug.Log("Test button clicked, but not implemented yet.");
            // comingSoonPanel.SetActive(true);
            // comingSoonButton.onClick.AddListener(() =>
            // {
            //     comingSoonPanel.SetActive(false);
            // });
            transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 10, 1).OnComplete(() =>
            {
                // Simpan nama stage, dan Load scene gameplay, tapi hanya pecel saja yang sudah ada
                if (GameData.ResepDipilih.namaResep == "Nasi Pecel")
                {
                    AudioManager.Instance.PlayMusic("gameplay");
                    GameData.GameType = GameType.SkillTest; // Set game type to SkillTest
                    SceneLoader.LoadScene("SkillTest");
                }
                else
                {
                    notAvailablePanel.SetActive(true);
                    notAvailableButton.onClick.AddListener(() =>
                    {
                        // Close not available panel
                        notAvailablePanel.SetActive(false);
                        // Reset selected recipe
                        GameData.ResepDipilih = null;
                        // Hide stage panel
                    });
                    stagePanel.SetActive(false);
                    // animate not available panel
                    notAvailablePanel.transform.localScale = Vector3.zero;
                    notAvailablePanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

                }
            });
        });

        practice.onClick.AddListener(() =>
        {
            // Debug.Log("Practice button clicked, but not implemented yet.");
            // comingSoonPanel.SetActive(true);
            // comingSoonButton.onClick.AddListener(() =>
            // {
            //     comingSoonPanel.SetActive(false);
            // });
            transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 10, 1).OnComplete(() =>
            {
                // Simpan nama stage, dan Load scene gameplay, tapi hanya pecel saja yang sudah ada
                if (GameData.ResepDipilih.namaResep == "Nasi Pecel")
                {
                    AudioManager.Instance.PlayMusic("gameplay");
                    GameData.GameType = GameType.Practice; // Set game type to Practice
                    SceneLoader.LoadScene("Story");
                }
                else
                {
                    notAvailablePanel.SetActive(true);
                    notAvailableButton.onClick.AddListener(() =>
                    {
                        // Close not available panel
                        notAvailablePanel.SetActive(false);
                        // Reset selected recipe
                        GameData.ResepDipilih = null;
                        // Hide stage panel
                    });
                    stagePanel.SetActive(false);
                    // animate not available panel
                    notAvailablePanel.transform.localScale = Vector3.zero;
                    notAvailablePanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

                }
            });
        });

        closePanelButton.onClick.AddListener(() =>
        {
            stagePanel.SetActive(false);
            GameData.ResepDipilih = null; // reset selected recipe
        });

        var pulauData = GameData.PulauDipilih;
        Debug.Log("Memuat Pulau: " + pulauData.namaPulau);
        if (pulauData == null)
        {
            Debug.LogError("Pulau tidak ditemukan di database.");
            return;
        }
        if (backgroundImage != null)
        {
            backgroundImage.sprite = pulauData.backgroundImage;
            // backgroundImage.SetNativeSize();
        }
        else
        {
            Debug.LogWarning("Background image not set in StageSelect.");
        }

        homeButton.onClick.AddListener(() =>
        {
            GameData.ResetData();
            SceneLoader.LoadScene("PulauSelect");
        });



        GenerateStageCards(pulauData);
    }

    void GenerateStageCards(PulauDataSO pulau)
    {

        stageCards = new StageCardUI[pulau.resepList.Length];
        for (int i = 0; i < pulau.resepList.Length; i++)
        {
            var card = Instantiate(stageCardPrefab, cardContainer);
            var ui = card.GetComponent<StageCardUI>();
            Debug.Log("Membuat kartu stage untuk: " + pulau.resepList[i].namaResep);
            ui.Init(pulau.resepList[i]);

            var rect = card.GetComponent<RectTransform>();
            var canvasGroup = card.GetComponent<CanvasGroup>();
            if (!canvasGroup) canvasGroup = card.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            rect.localScale = Vector3.one * 0.6f;

            // Animasi masuk: fade + scale-up
            canvasGroup.DOFade(1f, 0.4f).SetDelay(i * 0.05f);
            rect.DOScale(1f, 0.4f).SetDelay(i * 0.05f).SetEase(Ease.OutBack);

            stageCards[i] = ui;

        }

        // set scroll to the leftmost position
        var scrollRect = cardContainer.GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.horizontalNormalizedPosition = 0f;
        }
        else
        {
            Debug.LogWarning("ScrollRect not found in parent of cardContainer.");
        }


    }

    public void ShowPanel(ResepDataSO resep)
    {
        Debug.Log("Menampilkan panel untuk resep: " + resep.namaResep);
        if (resep == null)
        {
            Debug.LogError("Resep tidak boleh null saat menampilkan panel.");
            return;
        }

        GameData.ResepDipilih = resep;

        stagePanel.SetActive(true);
        titleImage.sprite = resep.resepText;
        resepImage.sprite = resep.ikonResep;

        // Set ukuran gambar resep
        Utilz.SetSizeNormalized(resepImage.GetComponent<RectTransform>(), resep.ikonResep, 350f, 350f);
        Utilz.SetSizeNormalized(titleImage.GetComponent<RectTransform>(), resep.resepText, 350f, 350f);

        int normalStars = ProgressManager.LoadStars(resep, GameType.Normal);
        int skillTestStarsCount = ProgressManager.LoadStars(resep, GameType.SkillTest);

        // 2. Perbarui tampilan bintang
        UpdateStarUI(normalModeStars, normalStars);
        UpdateStarUI(skillTestStars, skillTestStarsCount);

        // 3. Terapkan logika unlock untuk tombol
        // Tombol Uji Kemampuan (test) aktif jika mode Normal dapat 3 bintang.
        test.interactable = (normalStars == 3);

        // Tombol Uji Pemahaman (quiz) aktif jika mode Normal pernah selesai (minimal 1 bintang).
        quiz.interactable = (normalStars > 0);
    }
    
    private void UpdateStarUI(Image[] starImages, int activeCount)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            // Buat warna baru dari warna asli bintang
            Color starColor = starImages[i].color;

            if (i < activeCount)
            {
                // Jika bintang ini didapat, buat terlihat penuh (alpha = 1)
                starColor.a = 1f;
            }
            else
            {
                // Jika bintang ini belum didapat, buat redup (alpha = 0.3)
                starColor.a = 0.3f;
            }

            starImages[i].color = starColor;
        }
    }
}