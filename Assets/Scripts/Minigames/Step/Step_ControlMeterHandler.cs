using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[System.Serializable]
public class Step_ControlMeterHandler : Step
{
    [Header("UI References")]
    [SerializeField] private Slider controlMeter;
    [SerializeField] private Button controlButton; // visual only (optional)
    [SerializeField] private Slider progressBar; // optional, for visual feedback

    [Header("Meter Settings")]
    [SerializeField] private float cookingSpeed = 0.1f;
    [SerializeField] private float speedIncrement = 0.1f;
    [SerializeField] private float maxCookingSpeed = 0.5f;
    [SerializeField] private float minCookingSpeed = 0.1f;

    [Header("Target Range")]
    [SerializeField] private float lowerBound = 0.35f;
    [SerializeField] private float upperBound = 0.65f;
    [SerializeField] private float targetTime = 10f;

    private float elapsedTime = 0f;
    private bool isPressed = false;
    private float currentSpeed;
    private ResepDataSO resep { get; set; }


    void Start()
    {
        resep = GameData.ResepDipilih;
        Alat tombol = resep.langkahMasak[GameplayManager.Instance.CurrentStep].alatDiperlukan[0];

        controlButton.GetComponent<Image>().sprite = tombol.gambarAlat;

        isActive = true;
        if (controlMeter == null || controlButton == null)
        {
            Debug.LogError("Control Meter or Button is not assigned!");
            return;
        }

        if (progressBar != null)
        {
            progressBar.minValue = 0f;
            progressBar.maxValue = targetTime;
            progressBar.value = 0f;
        }

        controlMeter.minValue = 0f;
        controlMeter.maxValue = 1f;
        controlMeter.value = 0.5f;

        currentSpeed = cookingSpeed;

        if(resep.langkahMasak[GameplayManager.Instance.CurrentStep].alatDiperlukan.Length > 1)
        {
            InstantiateTutupBlender();
        }
    }

    private void InstantiateTutupBlender()
    {
        Alat tutupBlender = resep.langkahMasak[GameplayManager.Instance.CurrentStep].alatDiperlukan[1];
        GameObject tutupBlenderObj = Instantiate(tutupBlender,transform).gameObject;
        tutupBlenderObj.name = tutupBlender.namaAlat;
        tutupBlenderObj.GetComponent<Image>().sprite = tutupBlender.gambarAlat;
        Utilz.SetSizeNormalized(tutupBlenderObj.GetComponent<RectTransform>(), tutupBlender.gambarAlat, 500f, 500f);
        RectTransform rectTransform = tutupBlenderObj.GetComponent<RectTransform>();
        // Set the position of the tutup blender to be above the control button
        rectTransform.localPosition = new Vector3(0, 0, 0); // Adjust Y position as needed
    }

    public override void DisableStep()
    {
        isActive = false;
        controlMeter.value = 0f;
        elapsedTime = 0f;
        currentSpeed = cookingSpeed;
        isPressed = false;
        Debug.Log("Step disabled");
    }

    void OnEnable()
    {
        InputHandler.Instance.OnTouchStarted += StartPress;
        InputHandler.Instance.OnTouchCanceled += StopPress;
    }

    void OnDisable()
    {
        if (InputHandler.Instance == null) return;

        InputHandler.Instance.OnTouchStarted -= StartPress;
        InputHandler.Instance.OnTouchCanceled -= StopPress;
    }

    void Update()
    {
        if (!isActive) return;
        UpdatePressStatus();
        UpdateControlMeter();
    }

    private void StartPress(Vector2 pos)
    {
        isPressed = true;
        Debug.Log("Pressed down");
    }

    private void StopPress(Vector2 pos)
    {
        isPressed = false;
        Debug.Log("Released");
    }

    private void UpdatePressStatus()
    {
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            // Jika jari sedang menekan dan berada di atas tombol
            if (touch.press.isPressed && InButtonBounds(controlButton))
            {
                isPressed = true;
            }
            else
            {
                isPressed = false;
            }
        }
    }


    private void UpdateControlMeter()
    {
        if (elapsedTime >= targetTime)
        {
            return;
        }
        if (isPressed && InButtonBounds(controlButton))
        {
            controlMeter.value += currentSpeed * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed + speedIncrement * Time.deltaTime, maxCookingSpeed);
        }
        else
        {
            controlMeter.value -= currentSpeed * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed - speedIncrement * Time.deltaTime, minCookingSpeed);
        }

        controlMeter.value = Mathf.Clamp(controlMeter.value, 0f, 1f);

        if (controlMeter.value >= lowerBound && controlMeter.value <= upperBound)
        {
            elapsedTime += Time.deltaTime;
            if (progressBar != null)
            {
                progressBar.value = elapsedTime;
            }
            if (elapsedTime >= targetTime)
            {
                Debug.Log("Success! Meter stable for required time.");
                GameplayManager.Instance.NextStep();
            }
        }
        else
        {
            elapsedTime -= Time.deltaTime;
            if(elapsedTime < 0f)
            {
                elapsedTime = 0f; // Reset elapsed time if it goes negative
            }
            if (progressBar != null)
            {
                progressBar.value = elapsedTime;
            }
        }
    }

    private bool InButtonBounds(Button button)
    {
        Vector2 position = InputHandler.Instance.GetTouchPosition();
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        Vector2 touchPos = new Vector2(position.x - screenWidth / 2, position.y - screenHeight / 2);

        Vector2 buttonPos = rectTransform.anchoredPosition;
        Vector2 buttonSize = rectTransform.sizeDelta;
        Vector2 min = buttonPos - buttonSize / 2;
        Vector2 max = buttonPos + buttonSize / 2;
        Debug.Log("Touch Position: " + touchPos);
        Debug.Log($"Button Position: {buttonPos}, Size: {buttonSize}, Min: {min}, Max: {max}");
        return touchPos.x >= min.x && touchPos.x <= max.x && touchPos.y >= min.y && touchPos.y <= max.y;
    }
}
