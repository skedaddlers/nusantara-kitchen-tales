using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Step_Pound : Step
{
    public Image bahanDisplay;
    public Sprite[] bahanPoundImages;
    public Bahan bahanPound;
    public float timeToPound = 10f;
    public float timeForImageChange = 5f;
    public float minDistanceToPound = 10f; // Minimum pixel distance for valid pound

    private Vector2 lastTouchPosition;
    private int currentImageIndex = 0;
    private float elapsedTime = 0f;
    private float imageChangeTimer = 0f;
    private Tween tween;
    private bool isPounding = false;
    private RectTransform bahanRect;
    private Canvas canvas;
    private Camera uiCamera;

    void Start()
    {
        isActive = true;
        bahanRect = bahanDisplay.GetComponent<RectTransform>();
        
        // Get canvas and camera for proper coordinate conversion
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            uiCamera = canvas.worldCamera;
        }
    }

    void Update()
    {
        if (!isActive) return;
        
        // Update pounding progress if actively pounding
        if (isPounding)
        {
            elapsedTime += Time.deltaTime;
            imageChangeTimer += Time.deltaTime;
            
            // Change image based on progress
            if (imageChangeTimer >= timeForImageChange && currentImageIndex < bahanPoundImages.Length - 1)
            {
                currentImageIndex++;
                bahanDisplay.sprite = bahanPoundImages[currentImageIndex];
                imageChangeTimer = 0f;
            }
            
            // Check if pounding is complete
            if (elapsedTime >= timeToPound)
            {
                CompletePounding();
            }
        }
    }

    public override void DisableStep()
    {
        isActive = false;
        isPounding = false;
        elapsedTime = 0f;
        imageChangeTimer = 0f;
        currentImageIndex = 0;
        bahanDisplay.sprite = bahanPoundImages[0];
        bahanDisplay.rectTransform.localScale = Vector3.one;
        tween?.Kill();
        tween = null;
        Debug.Log("Step disabled");
    }

    private void Awake()
    {
        int currentStep = GameplayManager.Instance.CurrentStep;
        if (GameData.ResepDipilih.langkahMasak[currentStep].addToExisting)
        {
            bahanPound = GameData.ResepDipilih.langkahMasak[currentStep - 1].bahanDiperlukan[0];
            bahanPoundImages = GameData.ResepDipilih.langkahMasak[currentStep - 1].bahanDiperlukan[0].gambarBahanPound;
        }
        else
        {
            bahanPound = GameData.ResepDipilih.langkahMasak[currentStep].bahanDiperlukan[0];
            bahanPoundImages = bahanPound.gambarBahanPound;
        }
        
        bahanDisplay.sprite = bahanPoundImages[0];
        Utilz.SetSizeNormalized(bahanDisplay.rectTransform, bahanPoundImages[0], 500f, 500f);
    }

    void OnEnable()
    {
        InputHandler.Instance.OnTouchStarted += OnTouchStart;
        InputHandler.Instance.OnTouchMoved += OnTouchMove;
        InputHandler.Instance.OnTouchCanceled += OnTouchEnd;
    }

    void OnDisable()
    {
        if (InputHandler.Instance == null) return;
        InputHandler.Instance.OnTouchStarted -= OnTouchStart;
        InputHandler.Instance.OnTouchMoved -= OnTouchMove;
        InputHandler.Instance.OnTouchCanceled -= OnTouchEnd;
    }

    private void OnTouchStart(Vector2 touchPosition)
    {
        if (!isActive || isPounding) return;
        
        // Check if touch is on the bahan
        if (IsPointOverBahan(touchPosition))
        {
            StartPounding(touchPosition);
        }
    }

    private void OnTouchMove(Vector2 touchPosition)
    {
        if (!isActive || !isPounding) return;
        
        // Check if still over bahan
        // if (!IsPointOverBahan(touchPosition))
        // {
        //     StopPounding();
        //     return;
        // }
        
        // Check if movement is significant enough (pounding motion)
        float distance = Vector2.Distance(touchPosition, lastTouchPosition);
        if (distance >= minDistanceToPound)
        {
            // Valid pounding motion detected
            lastTouchPosition = touchPosition;
            
            // Restart or continue the pounding animation
            if (tween == null || !tween.IsActive())
            {
                tween = bahanDisplay.rectTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.15f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }
    }

    private void OnTouchEnd(Vector2 touchPosition)
    {
        if (!isActive) return;
        StopPounding();
    }

    private void StartPounding(Vector2 position)
    {
        Debug.Log($"Start pounding at: {position}");
        isPounding = true;
        lastTouchPosition = position;
        
        // Start animation
        if (tween == null || !tween.IsActive())
        {
            tween = bahanDisplay.rectTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.15f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    private void StopPounding()
    {
        if (!isPounding) return;
        
        Debug.Log("Stop pounding");
        isPounding = false;
        
        // Stop animation
        tween?.Kill();
        tween = null;
        bahanDisplay.rectTransform.localScale = Vector3.one;
    }

    private void CompletePounding()
    {
        Debug.Log("Pounding complete!");
        StopPounding();
        DisableStep();
        GameplayManager.Instance.NextStep();
    }

    private bool IsPointOverBahan(Vector2 screenPosition)
    {
        // Use RectTransformUtility for accurate UI hit testing
        return RectTransformUtility.RectangleContainsScreenPoint(
            bahanRect,
            screenPosition,
            uiCamera
        );
    }
}