using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIRaycaster : MonoBehaviour
{
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    public void RaycastUI(Vector2 screenPosition)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = screenPosition;
        
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        Debug.Log("Raycasted UI at: " + screenPosition + " with " + results.Count + " results.");   
        foreach (var result in results)
        {
            Debug.Log("UI Hit: " + result.gameObject.name);
            PulauSelector selector = result.gameObject.GetComponent<PulauSelector>();
            if (selector != null)
            {
                selector.OnPulauSelected();
            }
        }
        if (results.Count == 0)
        {
            UIPulauSelect.Instance.CloseInfoPanel();
        }
    }
}
