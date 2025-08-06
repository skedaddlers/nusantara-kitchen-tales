using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

// Step that represents the action of pounding an ingredient
// the player touch and drag on the ingredient image
// and the ingredient will be pounded 
// after some time the image will be replaced with the images in the array as the ingredient is pounded


public class Step_Pound : Step
{
    public Image bahanDisplay;
    public Sprite[] bahanPoundImages;
    public Bahan bahanPound;
    public float timeToPound = 10f;
    public float timeForImageChange = 5f;
    public float minDistanceToPound = 0.1f; // Minimum distance to consider as a pound action

    private Vector2 currentTouchPosition;
    private int currentImageIndex = 0;
    private float elapsedTime = 0f;
    private Tween tween;
    private bool isPounding = false;

    void Start()
    {
        isActive = true;
    }

    void Update()
    {
        UpdatePressStatus();
    }

    public override void DisableStep()
    {
        isActive = false;
        isPounding = false;
        elapsedTime = 0f;
        currentImageIndex = 0;
        bahanDisplay.sprite = bahanPoundImages[currentImageIndex];
        bahanDisplay.rectTransform.localScale = Vector3.one;
        tween?.Kill();
        tween = null;
        Debug.Log("Step disabled");
    }
    private void UpdatePressStatus()
    {
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            Vector2 touchPos = InputHandler.Instance.GetTouchPosition();
            if (touch.press.isPressed && InBounds(bahanDisplay.rectTransform, touchPos))
            {
                isPounding = true;
            }
            else
            {
                isPounding = false;

            }

        }
    }

    private void Awake()
    {
        int currentStep = GameplayManager.Instance.CurrentStep;
        if (GameData.ResepDipilih.langkahMasak[currentStep].addToExisting)
        {
            bahanPound = GameData.ResepDipilih.langkahMasak[currentStep - 1].bahanDiperlukan[0];
            bahanPoundImages = GameData.ResepDipilih.langkahMasak[currentStep - 1].bahanDiperlukan[0].gambarBahanPound;
            bahanDisplay.sprite = bahanPoundImages[currentImageIndex];
            return;
        }
        bahanPound = GameData.ResepDipilih.langkahMasak[currentStep].bahanDiperlukan[0];
        bahanPoundImages = bahanPound.gambarBahanPound;
        bahanDisplay.sprite = bahanPoundImages[currentImageIndex];
        Utilz.SetSizeNormalized(bahanDisplay.rectTransform, bahanPound.gambarBahanPound[0], 500f, 500f);


    }
    void OnEnable()
    {
        InputHandler.Instance.OnTouchStarted += StartPound;
        InputHandler.Instance.OnTouchMoved += ContinuePound;
        InputHandler.Instance.OnTouchCanceled += EndPound;
    }

    void OnDisable()
    {
        if (InputHandler.Instance == null) return;
        InputHandler.Instance.OnTouchStarted -= StartPound;
        InputHandler.Instance.OnTouchMoved -= ContinuePound;
        InputHandler.Instance.OnTouchCanceled -= EndPound;
    }

    private void StartPound(Vector2 touchPosition)
    {
        if (isPounding) return;
        Debug.Log(bahanDisplay.rectTransform.position);
        Debug.Log(bahanDisplay.rectTransform.rect);
        Debug.Log(bahanDisplay.rectTransform.rect.width);
        Debug.Log(bahanDisplay.rectTransform.rect.height);
        // Check if the touch is on the ingredient image
        if (InBounds(bahanDisplay.rectTransform, touchPosition))
        {
            Debug.Log($"StartPound: {touchPosition}");
            isPounding = true;
            // bahanDisplay.sprite = bahanPoundImages[currentImageIndex];
            currentTouchPosition = touchPosition;
        }
    }

    private void ContinuePound(Vector2 touchPosition)
    {
        Debug.Log($"ContinuePound: {touchPosition}" + $" isPounding: {isPounding}");
        if (!isPounding) return;

        // Check if the touch is still on the ingredient image
        if (InBounds(bahanDisplay.rectTransform, touchPosition))
        {
            float distance = Vector2.Distance(touchPosition, currentTouchPosition);
            Debug.Log($"Distance: {distance}");
            if (distance > minDistanceToPound)
            {
                // Update the current touch position
                currentTouchPosition = touchPosition;
            }
            else
            {
                // If the touch is too far from the last position, stop pounding
                // isPounding = false;
                return;
            }

            elapsedTime += Time.deltaTime;

            // Animate the ingredient image to show the pounding effect
            if (tween == null || !tween.IsActive())
            {
                tween = bahanDisplay.rectTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.15f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
            }

            // Change the image based on the elapsed time
            if (elapsedTime >= timeForImageChange)
            {
                if (currentImageIndex < bahanPoundImages.Length)
                {
                    bahanDisplay.sprite = bahanPoundImages[currentImageIndex];
                }
                currentImageIndex++;
            }

            // Check if the total time to pound has been reached
            if (elapsedTime >= timeToPound)
            {
                EndPound(touchPosition);
            }
        }
        else
        {
            isPounding = false;
            tween?.Kill();
            tween = null;
        }
    }

    private void EndPound(Vector2 touchPosition)
    {
        Debug.Log($"EndPound: {touchPosition}");    
        if (!isPounding) return;
        isPounding = false;
        tween?.Kill();
        tween = null;


        bahanDisplay.rectTransform.localScale = Vector3.one;
        if (elapsedTime >= timeToPound)
        {
            elapsedTime = 0f;
            GameplayManager.Instance.NextStep();
        }

    }
    
    private bool InBounds(RectTransform rectTransform, Vector2 touchPosition)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        Vector2 touchPos = new Vector2(touchPosition.x - screenWidth / 2, touchPosition.y - screenHeight / 2);

        Vector2 buttonPos = rectTransform.anchoredPosition;
        Vector2 buttonSize = rectTransform.sizeDelta;
        Vector2 min = buttonPos - buttonSize / 2;
        Vector2 max = buttonPos + buttonSize / 2;
        Debug.Log("Touch Position: " + touchPos);
        Debug.Log($"Button Position: {buttonPos}, Size: {buttonSize}, Min: {min}, Max: {max}");
        return touchPos.x >= min.x && touchPos.x <= max.x && touchPos.y >= min.y && touchPos.y <= max.y;

    }



}