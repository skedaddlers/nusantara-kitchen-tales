using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameplayTimer : MonoBehaviour
{
    public static GameplayTimer Instance { get; private set; }

    public float timeLimit = 20f; // Total time limit in seconds
    public GameObject stopwatchImage; // Image to represent the stopwatch
    public Image stopwatchNeedle; // Image to represent the stopwatch needle
    private float timeRemaining;
    private bool isRunning = false;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("GameplayTimer instance already exists. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }


        if (stopwatchNeedle == null)
        {
            Debug.LogError("GameplayTimer: stopwatchImage is not assigned!");
            return;
        }
        timeRemaining = timeLimit;
        UpdateStopwatchImage();
    }

    public void SetTimerVisibility(bool visible)
    {
        if (stopwatchNeedle != null)
        {
            // Cara paling sederhana adalah mengaktifkan/menonaktifkan GameObject dari gambar jarum.
            // Jika Anda memiliki parent object untuk semua UI timer, itu lebih baik.
            stopwatchImage.SetActive(visible);
        }
    }

    public void StartTimer()
    {
        if (!gameObject.activeInHierarchy || !stopwatchNeedle.gameObject.activeSelf) return;
        if (!isRunning)
        {
            isRunning = true;
            InvokeRepeating(nameof(UpdateTimer), 0f, 1f);
        }
    }
    private void UpdateTimer()
    {
        if (isRunning)
        {
            timeRemaining -= 1f;
            UpdateStopwatchImage();

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                isRunning = false;
                OnTimerEnd();
            }
        }
    }

    private void UpdateStopwatchImage()
    {
        if (stopwatchNeedle != null)
        {
            // set anchor to the bottom center
            RectTransform rectTransform = stopwatchNeedle.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0f);

            float fillAmount = timeRemaining / timeLimit;
            stopwatchNeedle.fillAmount = fillAmount;
            stopwatchNeedle.transform.rotation = Quaternion.Euler(0, 0, 360 * fillAmount);
        }
    }

    private void OnTimerEnd()
    {
        Debug.Log("Timer ended!");
        // Here you can add logic to handle what happens when the timer ends
        // For example, you might want to disable the current step or show a game over screen
        GameplayManager.Instance.OnTimerEnd();
        isRunning = false;
    }

    public void ResetTimer()
    {
        isRunning = false;
        timeRemaining = timeLimit;
        UpdateStopwatchImage();
        CancelInvoke(nameof(UpdateTimer));
    }

    public void PauseTimer()
    {
        if (isRunning)
        {
            isRunning = false;
            CancelInvoke(nameof(UpdateTimer));
        }
    }

}
