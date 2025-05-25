using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    public bool isPaused = false;
    public Transform stepParent;
    public TextMeshProUGUI stepText;
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


    private ResepDataSO resep;
    private int currentStep = 0;
    public int CurrentStep => currentStep;
    private List<GameObject> activeHandlers = new List<GameObject>();
    private Coroutine typingCoroutine;


    void Awake() => Instance = this;

    void Start()
    {
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
            SceneLoader.LoadScene("MainMenu");
        });
    }

    public void NextStep()
    {
        currentStep++;
        Debug.Log("Langkah berikutnya: " + currentStep);
        if (currentStep >= resep.langkahMasak.Length)
        {
            winPanel.gameObject.SetActive(true);
            Debug.Log("Resep selesai!");
        }
        else
        {
            ShowSuccess();
            StartCoroutine(NextStepRoutine());
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



    void LoadStep(int index)
    {

        SetMaskotImage("Happy");
        var step = resep.langkahMasak[index];
        Debug.Log("Memuat langkah: " + step.deskripsi + " ke indeks " + index);
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(step.deskripsi));
        stepImage.sprite = step.ikonStep;

        if (!step.addToExisting)
        {
            foreach (var handler in activeHandlers)
            {
                Destroy(handler);
            }
            activeHandlers.Clear();
        }

        if (step.prefabStep != null)
        {
            GameObject handler = Instantiate(step.prefabStep, stepParent);
            activeHandlers.Add(handler);
            handler.transform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.LogError("Prefab langkah masak tidak ditemukan!");
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
        for(int i = 0; i < maskotDescriptions.Length; i++)
        {
            if (maskotDescriptions[i].Equals(maskotName, System.StringComparison.OrdinalIgnoreCase))
            {
                maskotImage.sprite = maskotImages[i];
                return;
            }
        }
        Debug.LogError("Maskot tidak ditemukan: " + maskotName);
    }

    private void ShowSuccess()
    {
        // Show success animation or message
        Debug.Log("Langkah berhasil!");
        SetMaskotImage("Amazed");
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText("KAMU JAGO BGT BG"));
        maskotImage.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 1, 0.5f);
    }
    
}
