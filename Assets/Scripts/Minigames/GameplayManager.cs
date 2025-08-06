using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
public enum GameType
{
    None,
    Practice,
    Normal,
    SkillTest,
}
public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    public bool isPaused = false;
    public GameType gameType = GameType.Practice; // Default ke Practice
    public bool IsTimerActive => gameType != GameType.Practice;
    public bool AreRewardsEnabled => gameType != GameType.Practice;
    public bool ShowInstructions => gameType != GameType.SkillTest;
    public Transform stepParent;
    public GameObject instructionsPanel;
    public TextMeshProUGUI stepText;
    public TextMeshProUGUI pointsText;
    public Sprite[] maskotImages;
    public string[] maskotDescriptions = {
        "Sad",
        "Amazed",
        "Wink",
        "Happy",
        "Delicious"
    };
    public Image maskotImage;
    public GameObject winPanel;
    public Image stepImage;
    public Button pauseButton;
    public GameObject pauseMenu;
    public Button resumeButton;
    public Button homeButton;
    public Button homeButton2; // Duplicate button for home, if needed
    public GameObject starPrefab; // Prefab bintang untuk animasi
    public GameObject starContainer; // Tempat untuk menampung bintang-bintang
    [SerializeField]
    private int points = 0;
    public int Points => points;
    private ResepDataSO resep;
    [SerializeField]
    private int currentStep = 0;
    public int CurrentStep => currentStep;
    private List<GameObject> activeHandlers = new List<GameObject>();
    private Step currentStepHandler;
    public Step CurrentStepHandler => currentStepHandler;
    private Step previousStepHandler;
    public Step PreviousStepHandler => previousStepHandler;
    private Coroutine typingCoroutine;
    private bool isFailBefore = false;



    void Awake() => Instance = this;

    void Start()
    {
        if (pointsText != null)
        {
            pointsText.gameObject.SetActive(AreRewardsEnabled);
            if (AreRewardsEnabled)
            {
                pointsText.text = "Score: " + points;
            }
        }
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(ShowInstructions);
        }
        SetMaskotImage("Happy");
        resep = GameData.ResepDipilih;
        if (resep == null)
        {
            Debug.LogError("Resep tidak ditemukan!");
            return;
        }
        else
        {
            Debug.Log("Resep ditemukan: " + resep.langkahMasak.Length + " langkah");
            LoadStep(currentStep);
        }

        if (GameplayTimer.Instance != null)
        {
            GameplayTimer.Instance.SetTimerVisibility(IsTimerActive);
        }

        pauseButton.onClick.AddListener(() =>
        {
            PauseGame();
        });

        resumeButton.onClick.AddListener(() =>
        {
            ResumeGame();
        });

        homeButton.onClick.AddListener(() =>
        {
            // resume time
            Time.timeScale = 1f;

            GameData.ResetData();
            Debug.Log("Kembali ke menu utama");
            AudioManager.Instance.PlayMusic("background");
            SceneLoader.LoadScene("MainMenu");
        });

        homeButton2.onClick.AddListener(() =>
        {

            GameData.ResetData();
            Debug.Log("Kembali ke menu utama");
            AudioManager.Instance.PlayMusic("background");
            SceneLoader.LoadScene("MainMenu");
        });
    }

    public void NextStep()
    {
        currentStep++;
        Debug.Log("Langkah berikutnya: " + currentStep);
        if (AreRewardsEnabled)
        {
            if (!isFailBefore)
            {
                points += GameData.ResepDipilih.langkahMasak[currentStep - 1].pointsGiven;
            }
            pointsText.text = "Score: " + points;
        }
        if (isFailBefore)
        {
            isFailBefore = false;
        }
        

        pointsText.text = "Score: " + points;
        if (currentStep >= resep.langkahMasak.Length)
        {
            Win();
        }
        else
        {
            StartCoroutine(NextStepRoutine());
            if (IsTimerActive)
            {
                GameplayTimer.Instance.PauseTimer();
            }
        }
    }

    private IEnumerator NextStepRoutine()
    {
        ShowSuccess();

        // Tunggu hingga animasi DOTween selesai
        yield return new WaitForSeconds(2.5f);

        // Baru lanjut ke langkah berikutnya
        LoadStep(currentStep);
    }

    private IEnumerator FailStepRoutine()
    {
        ShowSad();

        yield return new WaitForSeconds(2.5f);

        LoadStep(currentStep, true); // Muat ulang langkah saat gagal

    }

    void LoadStep(int index, bool isRetry = false)
    {
        if(!ShowInstructions)
        {
            instructionsPanel.SetActive(false);
        }

        SetMaskotImage("Happy");
        var step = resep.langkahMasak[index];
        if (!step.addToExisting)
        {
            foreach (var handler in activeHandlers)
            {
                Destroy(handler);
            }
            activeHandlers.Clear();
        }
        if (currentStepHandler != null)
        {
            previousStepHandler = currentStepHandler;
            previousStepHandler.DisableStep();
        }
        else
        {
            previousStepHandler = null;
        }

        Debug.Log("Memuat langkah: " + step.deskripsi + " ke indeks " + index);
        if (ShowInstructions)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(step.deskripsi));
        }
        stepImage.sprite = step.ikonStep;


        if (step.prefabStep != null)
        {
            GameObject handler = Instantiate(step.prefabStep, stepParent);
            activeHandlers.Add(handler);
            handler.transform.localPosition = Vector3.zero;
            currentStepHandler = handler.GetComponent<Step>();
            if (isRetry)
            {
                // destroy previous step handler if it exists
                if (previousStepHandler != null)
                {
                    Destroy(previousStepHandler.gameObject);
                }
            }
        }
        else
        {
            Debug.LogError("Prefab langkah masak tidak ditemukan!");
        }

        if (IsTimerActive)
        {
            GameplayTimer.Instance.ResetTimer();
            GameplayTimer.Instance.StartTimer();
        }
    }

    private IEnumerator TypeText(string fullText)
    {
        stepText.text = "";
        foreach (char c in fullText)
        {
            stepText.text += c;
            yield return new WaitForSeconds(0.03f); // kecepatan ketik
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    private void SetMaskotImage(string maskotName)
    {
        for (int i = 0; i < maskotDescriptions.Length; i++)
        {
            if (maskotDescriptions[i].Equals(maskotName, System.StringComparison.OrdinalIgnoreCase))
            {
                maskotImage.sprite = maskotImages[i];
                return;
            }
        }
        Debug.LogError("Maskot tidak ditemukan: " + maskotName);
    }

    private void ShowSad()
    {
        // Show sad animation or message
        Debug.Log("Langkah gagal!");
        SetMaskotImage("Sad");
        if (ShowInstructions)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText("Coba lagi! tapi kamu tidak dapat poin :("));
        }
        else
        {
            instructionsPanel.SetActive(true);
            typingCoroutine = StartCoroutine(TypeText("Coba lagi!"));
        }
        maskotImage.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 1, 0.5f);
    }

    private void ShowSuccess()
    {
        if (!ShowInstructions)
        {
            instructionsPanel.SetActive(true);
        }
        // Show success animation or message
        Debug.Log("Langkah berhasil!");
        SetMaskotImage("Amazed");
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText("Yayyy! Kamu berhasil!"));
        maskotImage.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 1, 0.5f);
    }

    public void OnTimerEnd()
    {
        // Handle timer end logic
        Debug.Log("Waktu habis!");
        StartCoroutine(FailStepRoutine());
        isFailBefore = true;
    }

    private void Win()
    {
        if (IsTimerActive)
        {
            GameplayTimer.Instance.ResetTimer();
        }
        winPanel.gameObject.SetActive(true);
        winPanel.GetComponent<GIF>().Play();
        Debug.Log("Resep selesai!");
        // SetMaskotImage("Delicious");
        AnimateStars();
    }

    private void AnimateStars()
    {
        int maxPoints = 0;
        foreach (var step in resep.langkahMasak)
        {
            maxPoints += step.pointsGiven;
        }
        int count;
        float pointsPercentage = (float)points / maxPoints;
        if (pointsPercentage >= 0.8f)
        {
            count = 3;
        }
        else if (pointsPercentage >= 0.6f)
        {
            count = 2;
        }
        else
        {
            count = 1;
        }

        for (int i = 0; i < count; i++)
        {
            Debug.Log("Animasi bintang ke-" + (i + 1));
            GameObject star = Instantiate(starPrefab, starContainer.transform);
            star.transform.localScale = Vector3.zero;
            star.transform.localPosition = new Vector3(-300 + (i * 300), 20, 0f); // Adjust position based on count
            star.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(i * 0.2f);
            star.transform.DOLocalMoveY(50f, 0.5f).SetEase(Ease.OutBack).SetDelay(i * 0.2f);
        }
        ProgressManager.SaveStars(resep, gameType, count);
    }
    
    
}
