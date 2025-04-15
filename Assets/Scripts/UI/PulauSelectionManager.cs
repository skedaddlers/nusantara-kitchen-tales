using UnityEngine;

public class PulauSelectionManager : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private UIRaycaster uiRaycaster;

    private void Start()
    {
        cam = Camera.main;
        InputHandler.Instance.OnTouchPerformed += HandleTouch;
    }

    private void OnDestroy()
    {
        if (InputHandler.Instance != null)
            InputHandler.Instance.OnTouchPerformed -= HandleTouch;
    }

    void HandleTouch(Vector2 screenPosition)
    {
        uiRaycaster.RaycastUI(screenPosition);
    }

}
