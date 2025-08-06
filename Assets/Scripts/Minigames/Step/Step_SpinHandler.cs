using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Step_SpinGestureHandler : Step
{
    // public Image visualIndicator = null; // UI element to show the spin
    public Sprite spinSprite = null; // sprite for the visual indicator
    public float requiredSpin = 720f; // derajat total
    public float spinProgress = 0f;

    private Vector2 lastPos;
    private bool isDragging = false;
    public GameObject alatPrefab; // Prefab untuk alat yang akan di-instantiate
    private ResepDataSO resep { get; set; }

    private void Awake()
    {
        isActive = true;

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
            // Set posisi alat
            float posisiAlat = Screen.width / (alatDiperlukan.Length + 1);
            item.transform.localPosition = new Vector3(-150, -150, 0);
            alatPrefab = item.gameObject; // Set alatPrefab untuk digunakan nanti
        }


        // if (visualIndicator == null)
        // {
        //     Debug.LogError("Visual Indicator tidak di-set!");
        //     return;
        // }

        // if (spinSprite != null)
        // {
        //     visualIndicator.sprite = spinSprite;
        // }
        // else
        // {
        //     Debug.LogError("Spin Sprite tidak di-set!");
        // }
    }

    public override void DisableStep()
    {
        isActive = false;
        isDragging = false;
        spinProgress = 0f; // Reset progress
        lastPos = Vector2.zero; // Reset last position

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
        if (!isActive) return; // Ensure step is active
        Debug.Log("Drag started at: " + position);
        isDragging = true;
        lastPos = position;
    }

    private void EndDrag(Vector2 position)
    {
        if (!isDragging) return;
        Debug.Log("Drag ended at: " + position);
        isDragging = false;
        spinProgress = 0f; // Reset progress after drag ends
    }

    private void ContinueDrag(Vector2 position)
    {
        if (!isDragging) return;
        if (!isActive) return; // Ensure step is active

        Debug.Log("Dragging at: " + position);

        float centerX = Screen.width / 2;
        float centerY = Screen.height / 2;

        Vector2 center = new Vector2(centerX, centerY);
        // Get previous and current direction from center to touch position
        Vector2 prevDir = lastPos - center;
        Vector2 currDir = position - center;

        // Calculate signed angle between the two directions
        float angle = Vector2.SignedAngle(prevDir, currDir);

        // Add to spin progress
        spinProgress += angle;
        lastPos = position;

        // Rotate the visual indicator
        // visualIndicator.transform.Rotate(0, 0, angle); // negative if you want clockwise

        // Debug.Log("Spin progress: " + spinProgress);

        if (Mathf.Abs(spinProgress) >= requiredSpin)
        {
            // Debug.Log("Spin selesai!");
            DisableStep(); // Disable step after completing the spin
            GameplayManager.Instance.NextStep();
            EndDrag(position);
        }
        if (alatPrefab == null)
        {
            RotateBahan(angle); // Rotate the alat based on the spin angle
        }
        else
        {
            MoveAlat(angle); // Rotate the alat based on the spin angle
        }
    }

    private float alatAngle = 0f; // Keep track of cumulative angle for circular movement
    private float radius = 100f;  // Radius of circular motion, adjust as needed
    private Vector2 circleCenter; // Center of circular path

    private void MoveAlat(float angle)
    {
        if (alatPrefab == null) return; // Ensure alatPrefab is set

        // Update the circle center to the current position of the visual indicator
        circleCenter = new Vector2(transform.position.x - 150, transform.position.y -200); // Adjust Y offset as needed
        // Update the cumulative angle
        alatAngle += angle;

        // Calculate new position for the alat based on circular motion
        float x = circleCenter.x + radius * Mathf.Cos(alatAngle * Mathf.Deg2Rad);
        float y = circleCenter.y + radius * Mathf.Sin(alatAngle * Mathf.Deg2Rad);

        // Set the new position of the alat
        alatPrefab.transform.position = new Vector2(x, y);
    }

    private void RotateBahan(float angle)
    {
        // get the sibling game object of the Step_SpinGestureHandler, and then get the bahan inside it
        Transform sibling = transform.parent.GetChild(0);
        if (sibling == null) return; // Ensure sibling exists

        // find child with component Bahan
        Transform bahan = sibling.Find("Bahan");


        if (bahan == null) return; // Ensure bahan exists
                                   // Rotate the bahan based on the angle
        bahan.Rotate(0, 0, angle); // Rotate the bahan based on the spin angle

    }




}
