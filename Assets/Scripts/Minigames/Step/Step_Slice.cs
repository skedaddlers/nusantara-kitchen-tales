using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Step_Slice : Step
{
    public Image[] bahanDisplay;
    public Sprite[] bahanSliceImages;
    public Bahan bahanSlice;
    public float distanceSlice = 100f; // Minimum distance to consider as a slice action

    private Vector2 currentTouchPosition;
    private int totalSlices;
    private int slicesCount = 0;

    private void Awake()
    {
        int currentStep = GameplayManager.Instance.CurrentStep;
        if (GameData.ResepDipilih.langkahMasak[currentStep].addToExisting)
        {
            currentStep--;
        }
        bahanSlice = GameData.ResepDipilih.langkahMasak[currentStep].bahanDiperlukan[0];
        bahanSliceImages = bahanSlice.gambarBahanSlice;
        totalSlices = bahanSliceImages.Length - 1; // Total slices is the length of the array minus 1
        bahanDisplay = new Image[bahanSliceImages.Length];
        CreateBahan();
        PositionBahan();
    }

    private void PositionBahan()
    {
        float totalLength = 0;
        for (int i = 0; i < bahanDisplay.Length; i++)
        {
            totalLength += bahanDisplay[i].rectTransform.rect.width;
        }

        float startX = Screen.width / 2f - (totalLength / 2);
        for (int i = 0; i < bahanDisplay.Length; i++)
        {
            RectTransform rt = bahanDisplay[i].rectTransform;
            if (i > 0)
                rt.position = new Vector3(bahanDisplay[i - 1].rectTransform.position.x + bahanDisplay[i - 1].rectTransform.rect.width, rt.position.y, rt.position.z);
            else
                rt.position = new Vector3(startX + (bahanDisplay[i].rectTransform.rect.width / 2), rt.position.y, rt.position.z);
        }

    }



    private void CreateBahan()
    {
        RectTransform parent = GetComponent<RectTransform>();

        for (int i = 0; i < bahanSliceImages.Length; i++)
        {
            GameObject imgObj = new GameObject("SlicePart_" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imgObj.transform.SetParent(parent, false); // Make it a child of the canvas without world offset

            Image img = imgObj.GetComponent<Image>();
            img.sprite = bahanSliceImages[i];
            // img.SetNativeSize(); // Optional: size to match the sprite
            bahanDisplay[i] = img;
        }
    }


    void OnEnable()
    {
        InputHandler.Instance.OnTouchStarted += StartSlice;
        InputHandler.Instance.OnTouchMoved += ContinueSlice;
        InputHandler.Instance.OnTouchCanceled += EndSlice;
    }

    void OnDisable()
    {
        if (InputHandler.Instance == null) return;
        InputHandler.Instance.OnTouchStarted -= StartSlice;
        InputHandler.Instance.OnTouchMoved -= ContinueSlice;
        InputHandler.Instance.OnTouchCanceled -= EndSlice;
    }

    private void StartSlice(Vector2 touchPosition)
    {
        Debug.Log("StartSlice");
        if (RectTransformUtility.RectangleContainsScreenPoint(bahanDisplay[totalSlices - slicesCount].rectTransform, touchPosition))
        {
            currentTouchPosition = touchPosition;
        }
    }

    private void ContinueSlice(Vector2 touchPosition)
    {
        Debug.Log("ContinueSlice");
        if (RectTransformUtility.RectangleContainsScreenPoint(bahanDisplay[totalSlices - slicesCount].rectTransform, touchPosition))
        {
            // Calculate the distance between the current touch position and the previous one
            float distance = Vector2.Distance(touchPosition, currentTouchPosition);
            if (distance > distanceSlice)
            {
                // Update the current touch position
                currentTouchPosition = touchPosition;
                Slice();
            }
        }

    }
    private void Slice()
    {
        if (slicesCount < totalSlices)
        {

            // move the sliced image to the to the right smoothly
            Vector2 targetPosition = new Vector2(bahanDisplay[totalSlices - slicesCount].transform.position.x + 100, bahanDisplay[totalSlices - slicesCount].transform.position.y);
            bahanDisplay[totalSlices - slicesCount].transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutBack);
            // Check if all slices are done
            slicesCount++;
            if (slicesCount >= totalSlices)
            {
                Invoke("SliceComplete", 2f); // Wait for 2 seconds before calling SliceComplete
                // All slices are done, trigger the next step
            }
        }
    }

    private void EndSlice(Vector2 touchPosition)
    {
        Debug.Log("EndSlice");
        // Reset touch position
        currentTouchPosition = Vector2.zero;
    }
    
    private void SliceComplete()
    {
        GameplayManager.Instance.NextStep();
    }


}