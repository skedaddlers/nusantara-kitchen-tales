using UnityEngine;
using UnityEngine.UI;

public class Step_ControlMeterHandler : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider controlMeter;
    [SerializeField] private Button controlButton; // visual only (optional)

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

    void Start()
    {
        if (controlMeter == null || controlButton == null)
        {
            Debug.LogError("Control Meter or Button is not assigned!");
            return;
        }

        controlMeter.minValue = 0f;
        controlMeter.maxValue = 1f;
        controlMeter.value = 0.5f;

        currentSpeed = cookingSpeed;
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

    private void UpdateControlMeter()
    {
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

            if (elapsedTime >= targetTime)
            {
                Debug.Log("Success! Meter stable for required time.");
                GameplayManager.Instance.NextStep();
            }
        }
        else
        {
            elapsedTime = 0f;
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
