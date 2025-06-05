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
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.isPressed && touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                // if (IsPointerOverUI()) return;

                Vector2 pos = touch.position.ReadValue();
                OnTouchMoved?.Invoke(pos);
            }
        }
        else if (Mouse.current != null)
        {
            // Handle mouse input as a fallback
            if (Mouse.current.leftButton.isPressed)
            {
                // if (IsPointerOverUI()) return;

                Vector2 pos = Mouse.current.position.ReadValue();
                OnTouchMoved?.Invoke(pos);
            }
        }


        // if (InputActions.Gameplay.TouchPress.IsPressed())
        // {
        //     // if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        //     // {
        //     //     return;
        //     // }

        //     Vector2 pos = InputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
        //     OnTouchMoved?.Invoke(pos);
        // }

    }

    public Vector2 GetTouchPosition()
    {
        return InputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
    }



    private void OnEnable()
    {
        InputActions.Enable();
        InputActions.Gameplay.TouchPress.performed += OnTouchPressed;
        InputActions.Gameplay.TouchPress.canceled += OnTouchCancel;
        InputActions.Gameplay.TouchPress.started += OnTouchStart;
    
    }

    private void OnDisable()
    {
        if (InputActions == null) return;
        InputActions.Gameplay.TouchPress.performed -= OnTouchPressed;
        InputActions.Gameplay.TouchPress.canceled -= OnTouchCancel;
        InputActions.Gameplay.TouchPress.started -= OnTouchStart;
        InputActions.Disable();
    }

    private void OnTouchStart(InputAction.CallbackContext ctx)
    {
        Vector2 position = InputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
        // Debug.Log("Touch started at: " + position);
        // Misalnya spawn efek atau trigger game event

        OnTouchStarted?.Invoke(position);
    }

    private void OnTouchCancel(InputAction.CallbackContext ctx)
    {
        Vector2 position = InputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
        // Debug.Log("Touch canceled at: " + position);
        // Misalnya spawn efek atau trigger game event

        OnTouchCanceled?.Invoke(position);
    }

    private void OnTouchPressed(InputAction.CallbackContext ctx)
    {
        Vector2 position = InputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
        // Debug.Log("Touch at: " + position);
        // Misalnya spawn efek atau trigger game event
    
        OnTouchPerformed?.Invoke(position);
    }

    private void OnDestroy()
    {
        if (InputActions == null) return;
        InputActions.Disable();
    }

}
