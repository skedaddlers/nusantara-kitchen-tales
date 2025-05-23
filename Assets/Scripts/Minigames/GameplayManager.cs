using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    public bool isPaused = false;
    public Transform stepParent;
    public TextMeshProUGUI stepText;
    public Image stepImage;
    public Button pauseButton;
    public GameObject pauseMenu;
    public Button resumeButton;
    public Button homeButton;


    private ResepDataSO resep;
    private int currentStep = 0;
    public int CurrentStep => currentStep;
    private List<GameObject> activeHandlers = new List<GameObject>();


    void Awake() => Instance = this;

    void Start()
    {
        resep = GameData.ResepDipilih;
        if (resep == null)
        {
            Debug.LogError("Resep tidak ditemukan!");
            return;
        }
        else
        {
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
        if (currentStep >= resep.langkahMasak.Length)
        {
            Debug.Log("Resep selesai!");
        }
        else
        {
            LoadStep(currentStep);
        }
    }

    void LoadStep(int index)
    {

        var step = resep.langkahMasak[index];
        Debug.Log("Memuat langkah: " + step.deskripsi);
        stepText.text = step.deskripsi;
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
    
}
