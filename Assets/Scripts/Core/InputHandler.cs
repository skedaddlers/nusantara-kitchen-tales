using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private GameInputActions InputActions;
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

        InputActions = new GameInputActions();
        InputActions.Enable();
    }

    private void Update()
    {
        if (InputActions.Gameplay.TouchPress.IsPressed())
        {
            Vector2 pos = InputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
            OnTouchMoved?.Invoke(pos);
        }

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
        InputActions.Gameplay.TouchPress.performed -= OnTouchPressed;
        InputActions.Gameplay.TouchPress.canceled -= OnTouchCancel;
        InputActions.Gameplay.TouchPress.started -= OnTouchStart;
        InputActions.Disable();
    }

    private void OnTouchStart(InputAction.CallbackContext ctx)
    {
        Vector2 position = InputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
        Debug.Log("Touch started at: " + position);
        // Misalnya spawn efek atau trigger game event

        OnTouchStarted?.Invoke(position);
    }

    private void OnTouchCancel(InputAction.CallbackContext ctx)
    {
        Vector2 position = InputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
        Debug.Log("Touch canceled at: " + position);
        // Misalnya spawn efek atau trigger game event

        OnTouchCanceled?.Invoke(position);
    }

    private void OnTouchPressed(InputAction.CallbackContext ctx)
    {
        Vector2 position = InputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
        Debug.Log("Touch at: " + position);
        // Misalnya spawn efek atau trigger game event
    
        OnTouchPerformed?.Invoke(position);
    }

    private void OnDestroy()
    {
        InputActions.Disable();
    }

}
