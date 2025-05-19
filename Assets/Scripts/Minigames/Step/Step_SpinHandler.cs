using UnityEngine;
using UnityEngine.UI;

public class Step_SpinGestureHandler : MonoBehaviour
{
    public Image visualIndicator = null; // UI element to show the spin
    public Sprite spinSprite = null; // sprite for the visual indicator
    public float requiredSpin = 720f; // derajat total
    public float spinProgress = 0f;
    
    private Vector2 lastPos;
    private bool isDragging = false;

    private void Awake()
    {
        if (visualIndicator == null)
        {
            Debug.LogError("Visual Indicator tidak di-set!");
            return;
        }

        if (spinSprite != null)
        {
            visualIndicator.sprite = spinSprite;
        }
        else
        {
            Debug.LogError("Spin Sprite tidak di-set!");
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
        // Debug.Log("Drag started at: " + position);
        isDragging = true;
        lastPos = position;
    }

    private void EndDrag(Vector2 position)
    {
        // Debug.Log("Drag ended at: " + position);
        isDragging = false;
        spinProgress = 0f; // Reset progress after drag ends
    }

    private void ContinueDrag(Vector2 position)
    {
        if (!isDragging) return;

        // Debug.Log("Dragging at: " + position);

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
        visualIndicator.transform.Rotate(0, 0, angle); // negative if you want clockwise

        // Debug.Log("Spin progress: " + spinProgress);

        if (Mathf.Abs(spinProgress) >= requiredSpin)
        {
            // Debug.Log("Spin selesai!");
            GameplayManager.Instance.NextStep();
            EndDrag(position);
        }
    }



}
