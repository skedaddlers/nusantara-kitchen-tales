using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class Step_ControlMeterHandler : Step
{
    [Header("UI References")]
    [SerializeField] private Slider controlMeter;
    [SerializeField] private Button controlButton;
    [SerializeField] private Slider progressBar;

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
    private RectTransform buttonRect;
    private Canvas canvas;
    private Camera uiCamera;

    void Start()
    {
        resep = GameData.ResepDipilih;
        Alat tombol = resep.langkahMasak[GameplayManager.Instance.CurrentStep].alatDiperlukan[0];

        controlButton.GetComponent<Image>().sprite = tombol.gambarAlat;
        buttonRect = controlButton.GetComponent<RectTransform>();
        
        // Get canvas and camera for proper coordinate conversion
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            uiCamera = canvas.worldCamera; // Might be null for Screen Space - Overlay
        }

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
        GameObject tutupBlenderObj = Instantiate(tutupBlender, transform).gameObject;
        tutupBlenderObj.name = tutupBlender.namaAlat;
        tutupBlenderObj.GetComponent<Image>().sprite = tutupBlender.gambarAlat;
        Utilz.SetSizeNormalized(tutupBlenderObj.GetComponent<RectTransform>(), tutupBlender.gambarAlat, 500f, 500f);
        RectTransform rectTransform = tutupBlenderObj.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 0, 0);
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
        InputHandler.Instance.OnTouchStarted += OnTouchStart;
        InputHandler.Instance.OnTouchCanceled += OnTouchEnd;
    }

    void OnDisable()
    {
        if (InputHandler.Instance == null) return;
        InputHandler.Instance.OnTouchStarted -= OnTouchStart;
        InputHandler.Instance.OnTouchCanceled -= OnTouchEnd;
    }

    void Update()
    {
        if (!isActive) return;
        
        // Check if still pressing and update button state
        UpdateButtonPress();
        
        // Update the meter based on press state
        UpdateControlMeter();
    }

    private void OnTouchStart(Vector2 screenPos)
    {
        if (!isActive) return;
        
        if (IsPointOverButton(screenPos))
        {
            isPressed = true;
            Debug.Log("Button pressed");
        }
    }

    private void OnTouchEnd(Vector2 screenPos)
    {
        isPressed = false;
        Debug.Log("Button released");
    }

    private void UpdateButtonPress()
    {
        // Check if we're still touching and still over the button
        if (isPressed && InputHandler.Instance != null)
        {
            if (InputHandler.Instance.IsTouching())
            {
                Vector2 currentPos = InputHandler.Instance.GetTouchPosition();
                if (!IsPointOverButton(currentPos))
                {
                    isPressed = false; // Moved off button
                }
            }
            else
            {
                isPressed = false; // No longer touching
            }
        }
    }

    private bool IsPointOverButton(Vector2 screenPosition)
    {
        // Use RectTransformUtility for accurate UI hit testing
        return RectTransformUtility.RectangleContainsScreenPoint(
            buttonRect, 
            screenPosition, 
            uiCamera
        );
    }

    private void UpdateControlMeter()
    {
        if (elapsedTime >= targetTime)
        {
            return;
        }

        // Update meter value based on press state
        if (isPressed)
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

        // Check if meter is in target range
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
                DisableStep();
                GameplayManager.Instance.NextStep();
            }
        }
        else
        {
            // Decrease progress when out of range
            elapsedTime = Mathf.Max(0f, elapsedTime - Time.deltaTime);
            if (progressBar != null)
            {
                progressBar.value = elapsedTime;
            }
        }
    }
}