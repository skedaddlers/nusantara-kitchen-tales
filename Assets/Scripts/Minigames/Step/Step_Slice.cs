using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Step_Slice : Step
{
    public Image[] bahanDisplay;
    public Sprite[] bahanSliceImages;
    public Bahan bahanSlice;
    public float distanceSlice = 100f; // Minimum swipe distance to trigger slice

    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private int totalSlices;
    private int slicesCount = 0;
    private bool isSlicing = false;
    private Canvas canvas;
    private Camera uiCamera;

    private void Awake()
    {
        isActive = true;
        
        // Get canvas and camera
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            uiCamera = canvas.worldCamera;
        }
        
        int currentStep = GameplayManager.Instance.CurrentStep;
        if (GameData.ResepDipilih.langkahMasak[currentStep].addToExisting)
        {
            currentStep--;
        }
        
        bahanSlice = GameData.ResepDipilih.langkahMasak[currentStep].bahanDiperlukan[0];
        bahanSliceImages = bahanSlice.gambarBahanSlice;
        totalSlices = bahanSliceImages.Length - 1;
        bahanDisplay = new Image[bahanSliceImages.Length];
        
        CreateBahan();
        PositionBahan();
    }

    public override void DisableStep()
    {
        isActive = false;
        isSlicing = false;
        startTouchPosition = Vector2.zero;
        currentTouchPosition = Vector2.zero;
        slicesCount = 0;
    }

    private void PositionBahan()
    {
        float totalWidth = 0;
        for (int i = 0; i < bahanDisplay.Length; i++)
        {
            totalWidth += bahanDisplay[i].rectTransform.rect.width;
        }

        // Center the bahan pieces
        float startX = -totalWidth / 2f;
        float currentX = startX;
        
        for (int i = 0; i < bahanDisplay.Length; i++)
        {
            RectTransform rt = bahanDisplay[i].rectTransform;
            float halfWidth = rt.rect.width / 2f;
            rt.anchoredPosition = new Vector2(currentX + halfWidth, 0);
            currentX += rt.rect.width;
        }
    }

    private void CreateBahan()
    {
        RectTransform parent = GetComponent<RectTransform>();

        for (int i = 0; i < bahanSliceImages.Length; i++)
        {
            GameObject imgObj = new GameObject("SlicePart_" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imgObj.transform.SetParent(parent, false);

            Image img = imgObj.GetComponent<Image>();
            img.sprite = bahanSliceImages[i];
            bahanDisplay[i] = img;

            RectTransform rt = imgObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(bahanSliceImages[i].rect.width, bahanSliceImages[i].rect.height);
            Utilz.SetSizeNormalized(rt, bahanSliceImages[i], 500f, 500f);
        }
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
        if (!isActive || slicesCount >= totalSlices) return;
        
        // Check if touch is on the current slice piece
        int currentSliceIndex = totalSlices - slicesCount;
        if (IsPointOverImage(bahanDisplay[currentSliceIndex], touchPosition))
        {
            isSlicing = true;
            startTouchPosition = touchPosition;
            currentTouchPosition = touchPosition;
            Debug.Log($"Start slicing piece {currentSliceIndex}");
        }
    }

    private void OnTouchMove(Vector2 touchPosition)
    {
        if (!isActive || !isSlicing) return;
        
        currentTouchPosition = touchPosition;
        
        // Check if we've swiped far enough to trigger a slice
        float swipeDistance = Vector2.Distance(startTouchPosition, currentTouchPosition);
        
        if (swipeDistance >= distanceSlice)
        {
            PerformSlice();
            isSlicing = false; // Reset for next slice
        }
    }

    private void OnTouchEnd(Vector2 touchPosition)
    {
        if (!isActive || !isSlicing) return;
        
        // Check if the swipe was long enough even if finger lifted
        float swipeDistance = Vector2.Distance(startTouchPosition, touchPosition);
        
        if (swipeDistance >= distanceSlice)
        {
            PerformSlice();
        }
        
        isSlicing = false;
        startTouchPosition = Vector2.zero;
        currentTouchPosition = Vector2.zero;
    }

    private void PerformSlice()
    {
        if (slicesCount >= totalSlices) return;
        
        int currentSliceIndex = totalSlices - slicesCount;
        
        Debug.Log($"Slicing piece {currentSliceIndex}");
        
        // Calculate slice animation direction based on swipe
        Vector2 swipeDirection = (currentTouchPosition - startTouchPosition).normalized;
        float slideDistance = 150f;
        
        // Animate the sliced piece moving away
        Vector2 targetPosition = bahanDisplay[currentSliceIndex].rectTransform.anchoredPosition;
        targetPosition += swipeDirection * slideDistance;
        
        bahanDisplay[currentSliceIndex].rectTransform
            .DOAnchorPos(targetPosition, 0.5f)
            .SetEase(Ease.OutBack);
        
        // Optional: Add rotation for more dynamic effect
        float rotationAngle = Random.Range(-30f, 30f);
        bahanDisplay[currentSliceIndex].transform
            .DORotate(new Vector3(0, 0, rotationAngle), 0.5f)
            .SetEase(Ease.OutBack);
        
        slicesCount++;
        
        // Check if all slices are complete
        if (slicesCount >= totalSlices)
        {
            Invoke("SliceComplete", 0.6f); // Wait for animation to finish
        }
    }

    private void SliceComplete()
    {
        Debug.Log("All slices complete!");
        DisableStep();
        GameplayManager.Instance.NextStep();
    }

    private bool IsPointOverImage(Image image, Vector2 screenPosition)
    {
        if (image == null) return false;
        
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform,
            screenPosition,
            uiCamera
        );
    }
}