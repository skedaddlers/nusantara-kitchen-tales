using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private GameInputActions InputActions;
    public static InputHandler Instance { get; private set; }

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

    private void OnEnable()
    {
        InputActions.Enable();
        InputActions.Gameplay.TouchPress.performed += OnTouchPressed;
    }

    private void OnDisable()
    {
        InputActions.Disable();
    }

    private void OnTouchPressed(InputAction.CallbackContext ctx)
    {
        Vector2 position = InputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
        Debug.Log("Touch at: " + position);
        // Misalnya spawn efek atau trigger game event
    }

    private void OnDestroy()
    {
        InputActions.Disable();
    }

}
