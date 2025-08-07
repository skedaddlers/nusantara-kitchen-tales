using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Step_SpinGestureHandler : Step
{
    public Sprite spinSprite = null;
    public float requiredSpin = 720f; // Total degrees required
    public float spinProgress = 0f;

    private Vector2 lastPos;
    private bool isDragging = false;
    private GameObject alatPrefab;
    private ResepDataSO resep { get; set; }
    
    // For circular motion
    private float alatAngle = 0f;
    private float radius = 100f;
    private Vector2 circleCenter;
    private RectTransform canvasRect;

    private void Awake()
    {
        isActive = true;
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        resep = GameData.ResepDipilih;
        var alatDiperlukan = resep.langkahMasak[GameplayManager.Instance.CurrentStep].alatDiperlukan;
        
        if (alatDiperlukan == null)
        {
            Debug.LogError("Alat tidak ditemukan untuk langkah ini!");
            return;
        }
        
        foreach (var alat in alatDiperlukan)
        {
            if (alat == null)
            {
                Debug.LogError("Alat tidak ditemukan dalam resep!");
                return;
            }
            
            // Instantiate alat
            Alat item = Instantiate(alat, transform);
            item.name = alat.namaAlat;
            item.GetComponent<Image>().sprite = alat.gambarAlat;
            Utilz.SetSizeNormalized(item.GetComponent<RectTransform>(), alat.gambarAlat, 500f, 500f);
            
            // Set initial position
            item.transform.localPosition = new Vector3(-150, -150, 0);
            alatPrefab = item.gameObject;
        }
        
        // Set circle center for rotation
        circleCenter = transform.position;
    }

    public override void DisableStep()
    {
        isActive = false;
        isDragging = false;
        spinProgress = 0f;
        lastPos = Vector2.zero;
        Debug.Log("Step spin disabled");
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
        if (!isActive) return;
        
        // Check if touch is within reasonable bounds (optional)
        isDragging = true;
        lastPos = position;
        Debug.Log("Spin started at: " + position);
    }

    private void EndDrag(Vector2 position)
    {
        if (!isDragging) return;
        
        Debug.Log("Spin ended at: " + position);
        isDragging = false;
        // Don't reset progress here - keep it accumulated
    }

    private void ContinueDrag(Vector2 position)
    {
        if (!isDragging || !isActive) return;

        // Get center of screen
        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        
        // Calculate angle between last position and current position relative to center
        Vector2 prevDir = (lastPos - center).normalized;
        Vector2 currDir = (position - center).normalized;
        
        // Calculate signed angle
        float angle = Vector2.SignedAngle(prevDir, currDir);
        
        // Filter out noise (very small movements)
        if (Mathf.Abs(angle) < 0.1f) return;
        
        // Add to spin progress
        spinProgress += angle;
        lastPos = position;
        
        Debug.Log($"Spin angle: {angle:F2}, Total progress: {spinProgress:F2}/{requiredSpin}");
        
        // Move or rotate based on whether we have alat
        if (alatPrefab != null)
        {
            MoveAlat(angle);
        }
        else
        {
            RotateBahan(angle);
        }
        
        // Check if spin is complete
        if (Mathf.Abs(spinProgress) >= requiredSpin)
        {
            Debug.Log("Spin completed!");
            DisableStep();
            GameplayManager.Instance.NextStep();
        }
    }

    private void MoveAlat(float angle)
    {
        if (alatPrefab == null) return;
        
        // Update cumulative angle
        alatAngle += angle;
        
        // Calculate position on circle
        float radians = alatAngle * Mathf.Deg2Rad;
        float x = radius * Mathf.Cos(radians);
        float y = radius * Mathf.Sin(radians);
        
        // Set local position relative to parent
        alatPrefab.transform.localPosition = new Vector3(x - 150, y - 150, 0);
        
        // Optional: Rotate the tool itself as it moves
        // alatPrefab.transform.localRotation = Quaternion.Euler(0, 0, -alatAngle);
    }

    private void RotateBahan(float angle)
    {
        // Find bahan in sibling
        Transform sibling = transform.parent.GetChild(0);
        if (sibling == null) return;
        
        Transform bahan = sibling.Find("Bahan");
        if (bahan == null) return;
        
        // Rotate the bahan
        bahan.Rotate(0, 0, angle);
    }
}