using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Step_DragDropHandler : Step
{
    public int totalItems;
    public int itemsDroppedCorrectly;
    public int currentStep;
    public Bahan[] bahanDragDrop;
    private ResepDataSO resep { get; set; }

    private void Awake()
    {
        resep = GameData.ResepDipilih;
        currentStep = GameplayManager.Instance.CurrentStep;
        if (resep == null)
        {
            Debug.LogError("Resep tidak ditemukan!");
            return;
        }
        else
        {
            totalItems = resep.langkahMasak[currentStep].bahanDiperlukan.Length;
            bahanDragDrop = resep.langkahMasak[currentStep].bahanDiperlukan;
        }
    }

    // Load bahan yang diperlukan untuk langkah ini
    private void Start()
    {
        isActive = true;
        LoadAlat();
        LoadBahan();
    }

    public override void DisableStep()
    {
        isActive = false;
        itemsDroppedCorrectly = 0;
        currentStep = 0;
        Debug.Log("Step disabled");

        // Disable all draggable items
        foreach (var item in GetComponentsInChildren<DraggableItem>())
        {
            item.DisableDraggable();
            Debug.Log("Draggable item disabled: " + item.name);
        }
        
    }

    public virtual void LoadBahan()
    {
        float spacing = 400f;
        float startX = -((bahanDragDrop.Length - 1) * spacing) / 2f;
        // jika lebih dari 5, startx harus diubah
        if (bahanDragDrop.Length > 5)
        {
            startX = -((4 * spacing) / 2f);
        }

        for (int i = 0; i < bahanDragDrop.Length; i++)
        {
            Bahan prefab = bahanDragDrop[i];
            Bahan item = Instantiate(prefab, transform); // pastikan ini prefab UI
            RectTransform rt = item.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(startX + spacing * i, 250f); // adjust Y jika perlu

            item.name = prefab.namaBahan;
            item.GetComponent<Image>().sprite = prefab.gambarBahan;
            Utilz.SetSizeNormalized(rt, prefab.gambarBahan);

            // jika i lebih dari 5, atur posisi Y ke -100
            if (i == 5)
            {
                rt.anchoredPosition = new Vector2(startX + spacing * 4, 250f - 1 * 300f);
            }
            if (i == 6)
            {
                rt.anchoredPosition = new Vector2(startX, 250f - 1 * 300f);
            }
        }
    }

    public virtual void LoadAlat()
    {
        // Ambil semua alat yang diperlukan untuk langkah ini
        var alatDiperlukan = resep.langkahMasak[currentStep].alatDiperlukan;

        // posisi alat adalah lebar layar dibagi jumlah alat
        float posisiAlat = Screen.width / 2;
        int index = 0;
        foreach (var alat in alatDiperlukan)
        {
            // Instantiate prefab alat
            Alat item = Instantiate(alat, transform);
            item.transform.localPosition = new Vector3(0, -100f, 0);
            item.name = alat.namaAlat;
            item.GetComponent<DropTarget>().expectedBahanPrefab = bahanDragDrop;
            item.GetComponent<Image>().sprite = alat.gambarAlat;
            Utilz.SetSizeNormalized(item.GetComponent<RectTransform>(), alat.gambarAlat, 500f, 500f);
            index++;
        }
    }


    public void NotifyCorrectDrop()
    {
        if (!isActive) return;
        itemsDroppedCorrectly++;
        if (itemsDroppedCorrectly >= totalItems)
        {
            
            Debug.Log("Semua item sudah dijatuhkan dengan benar!");
            // Panggil fungsi untuk melanjutkan ke langkah berikutnya
            GameplayManager.Instance.NextStep();
        }
    }
    
}