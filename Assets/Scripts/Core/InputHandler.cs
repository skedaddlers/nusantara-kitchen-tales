using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    public GameInputActions InputActions;
    public static InputHandler Instance { get; private set; }

    public event System.Action<Vector2> OnTouchPerformed;
    public event System.Action<Vector2> OnTouchStarted;
    public event System.Action<Vector2> OnTouchCanceled;
    public event System.Action<Vector2> OnTouchMoved;

    private bool isTouching = false;
    private Vector2 lastTouchPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (InputActions == null)
            InputActions = new GameInputActions();

        InputActions.Enable();
    }

    private void Update()
    {
        // Unified touch/mouse handling
        Vector2? currentPosition = null;
        bool isPressed = false;

        // Check for touch input first (for mobile)
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.isPressed)
            {
                currentPosition = touch.position.ReadValue();
                isPressed = true;
            }
        }
        // Fallback to mouse input (for editor/PC)
        else if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                currentPosition = Mouse.current.position.ReadValue();
                isPressed = true;
            }
        }

        // Handle input state changes
        if (isPressed && currentPosition.HasValue)
        {
            if (!isTouching)
            {
                // Touch just started
                isTouching = true;
                lastTouchPosition = currentPosition.Value;
                OnTouchStarted?.Invoke(lastTouchPosition);
            }
            else if (Vector2.Distance(currentPosition.Value, lastTouchPosition) > 0.1f)
            {
                // Touch moved
                lastTouchPosition = currentPosition.Value;
                OnTouchMoved?.Invoke(lastTouchPosition);
            }
        }
        else if (isTouching && !isPressed)
        {
            // Touch just ended
            isTouching = false;
            OnTouchCanceled?.Invoke(lastTouchPosition);
        }
    }

    public Vector2 GetTouchPosition()
    {
        // Priority: Touch > Mouse > Last known position
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.isPressed)
                return touch.position.ReadValue();
        }
        
        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();
            
        return lastTouchPosition;
    }

    public bool IsTouching()
    {
        return isTouching;
    }

    private void OnEnable()
    {
        InputActions?.Enable();
    }

    private void OnDisable()
    {
        InputActions?.Disable();
    }

    private void OnDestroy()
    {
        InputActions?.Disable();
    }
}