using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIRaycaster : MonoBehaviour
{
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    private PulauSelector currentPulauSelector = null;

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
                if (currentPulauSelector == null || currentPulauSelector != selector)
                {
                    selector.OnPulauSelected();
                    currentPulauSelector = selector;
                }                
            }
            else if (currentPulauSelector != null)
            {
                currentPulauSelector = null;
            }
        }
        if (results.Count == 0)
        {
            UIPulauSelect.Instance.CloseInfoPanel();
        }
    }
}
