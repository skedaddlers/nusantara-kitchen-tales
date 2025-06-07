using UnityEngine;
using UnityEngine.EventSystems;

public class DropTarget : MonoBehaviour, IDropHandler
{
    public Bahan[] expectedBahanPrefab;
    public Step_DragDropHandler stepHandler;

    public void Start()
    {
        stepHandler = GetComponentInParent<Step_DragDropHandler>();
        if (stepHandler == null)
        {
            Debug.LogError("Step_DragDropHandler tidak ditemukan di parent!");
            return;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem item = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (item == null || stepHandler == null) return;

        for (int i = 0; i < expectedBahanPrefab.Length; i++)
        {
            if (item.gameObject.name == expectedBahanPrefab[i].name)
            {
                Debug.Log("Item dropped: " + item.gameObject.name);
                Debug.Log("Expected item: " + expectedBahanPrefab[i].name);
                item.droppedCorrectly = true;
                item.transform.SetParent(transform);
                item.transform.localPosition = Vector3.zero;
                stepHandler.NotifyCorrectDrop();
                break;
            }
        }
    }

    
}
