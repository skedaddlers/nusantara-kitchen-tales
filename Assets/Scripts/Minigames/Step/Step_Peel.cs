using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// For peeling potatoes or other vegetables
[System.Serializable]
public class Step_Peel : Step
{

    public Image[] bahanDisplay;
    public Sprite[] bahanSkinImages;
    public Sprite[] bahanPeelImages;
    public Bahan bahanPeel;
    public float distancePeel = 100f; // Minimum distance to consider as a peel action

    private Vector2 currentTouchPosition;
    private int totalPeels;
    private int peelsCount = 0;
    private int currentPeelIndex = 0;
    private bool[] peelCompleted; // Track which peels have been completed

    private void Awake()
    {
        int currentStep = GameplayManager.Instance.CurrentStep;
        if (GameData.ResepDipilih.langkahMasak[currentStep].addToExisting)
        {
            currentStep--;
        }
        bahanPeel = GameData.ResepDipilih.langkahMasak[currentStep].bahanDiperlukan[0];
        bahanSkinImages = bahanPeel.gambarBahanSkin;
        bahanPeelImages = bahanPeel.gambarBahanPeel;
        totalPeels = bahanPeelImages.Length; // Total peels is the length of the array minus 1
        bahanDisplay = new Image[bahanPeelImages.Length];
        peelCompleted = new bool[bahanPeelImages.Length];
        for (int i = 0; i < peelCompleted.Length; i++)
        {
            peelCompleted[i] = false; // Initialize all peels as not completed
        }
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

        for (int i = 0; i < bahanPeelImages.Length; i++)
        {
            GameObject imgObj = new GameObject("PeelPart_" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imgObj.transform.SetParent(parent, false); // Make it a child of the canvas without world offset
            Image img = imgObj.GetComponent<Image>();
            img.sprite = bahanSkinImages[i];
            // img.preserveAspect = true;
            // img.rectTransform.sizeDelta = new Vector2(100, 100); // Set size as needed
            bahanDisplay[i] = img;
        }
    }


    void OnEnable()
    {
        InputHandler.Instance.OnTouchStarted += StartDrag;
        InputHandler.Instance.OnTouchMoved += ContinueDrag;
        InputHandler.Instance.OnTouchCanceled += EndDrag;
    }

    void OnDisable()
    {
        if (InputHandler.Instance == null) return;
        InputHandler.Instance.OnTouchStarted -= StartDrag;
        InputHandler.Instance.OnTouchMoved -= ContinueDrag;
        InputHandler.Instance.OnTouchCanceled -= EndDrag;
    }

    private void StartDrag(Vector2 position)
    {
        Debug.Log("Start Drag at: " + position);
        for (int i = 0; i < bahanDisplay.Length; i++)
        {
            if (peelCompleted[i])
            {   
                continue; // If this peel is already completed, do nothing
            }
            if (RectTransformUtility.RectangleContainsScreenPoint(bahanDisplay[i].rectTransform, position))
            {
                currentTouchPosition = position;
                currentPeelIndex = i;
                break; // Start dragging the first matched peel
            }
        }


    }

    private void EndDrag(Vector2 position)
    {   
        Debug.Log("End Drag at: " + position);
        // Reset the current touch position
        currentTouchPosition = Vector2.zero;

    }

    private void ContinueDrag(Vector2 pos)
    {
        Debug.Log("Continue Drag at: " + pos + " for peel index: " + currentPeelIndex);

            // Debug.Log("Dragging within the peel area: " + currentPeelIndex);
            float distance = Vector2.Distance(pos, currentTouchPosition);
            Debug.Log("Distance: " + distance + " for peel index: " + currentPeelIndex);
            if (distance > distancePeel)
            {
                // Update the current touch position
                currentTouchPosition = pos;
                Peel();
            }
        
    }

    private void Peel()
    {
        if(peelCompleted[currentPeelIndex])
        {
            Debug.Log("This peel is already completed.");
            return; // If this peel is already completed, do nothing
        }
        if (peelsCount < totalPeels)
        {
            Debug.Log("Peeling: " + currentPeelIndex);
            bahanDisplay[currentPeelIndex].sprite = bahanPeelImages[currentPeelIndex];
            peelCompleted[currentPeelIndex] = true;
            peelsCount++;


            if (peelsCount >= totalPeels)
            {
                Invoke("PeelComplete", 1f);
            }

        }
    }

    private void PeelComplete()
    {
        Debug.Log("Peel Complete");
        // Here you can trigger the next step or any other action
        GameplayManager.Instance.NextStep();
    }
    

    

}   