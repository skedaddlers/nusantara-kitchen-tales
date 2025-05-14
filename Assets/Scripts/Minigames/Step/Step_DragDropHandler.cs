using UnityEngine;
using UnityEngine.UI;

public class Step_DragDropHandler : MonoBehaviour
{
    public int totalItems;
    public int itemsDroppedCorrectly;
    public int currentStep;
    public Bahan[] bahanDragDrop;
    private ResepDataSO resep {get; set; }

    private void Awake()
    {
        resep = GameData.ResepDipilih;
        currentStep = GameplayManager.Instance.CurrentStep;
        if(resep == null)
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
        LoadBahan();
        LoadAlat();
    }

    private void LoadBahan()
    {
        float spacing = 200f;
        float startX = -((bahanDragDrop.Length - 1) * spacing) / 2f;

        for (int i = 0; i < bahanDragDrop.Length; i++)
        {
            Bahan prefab = bahanDragDrop[i];
            Bahan item = Instantiate(prefab, transform); // pastikan ini prefab UI
            RectTransform rt = item.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(startX + spacing * i, -150f); // adjust Y jika perlu
            item.name = prefab.namaBahan;
            item.GetComponent<Image>().sprite = prefab.gambarBahan;
        }
    }

    private void LoadAlat()
    {
        // Ambil semua alat yang diperlukan untuk langkah ini
        var alatDiperlukan = resep.langkahMasak[currentStep].alatDiperlukan;

        // posisi alat adalah lebar layar dibagi jumlah alat
        float posisiAlat = Screen.width / (alatDiperlukan.Length + 1);
        int index = 0;
        foreach (var alat in alatDiperlukan)
        {
            // Instantiate prefab alat
            Alat item = Instantiate(alat, transform);
            item.transform.localPosition = new Vector3(posisiAlat * index, 0, 0);
            item.name = alat.namaAlat;
            item.GetComponent<DropTarget>().expectedBahanPrefab = bahanDragDrop;
            item.GetComponent<Image>().sprite = alat.gambarAlat;
            index++;
        }
    }


    public void NotifyCorrectDrop()
    {
        itemsDroppedCorrectly++;
        if (itemsDroppedCorrectly >= totalItems)
        {
            Debug.Log("Semua item sudah dijatuhkan dengan benar!");
            // Panggil fungsi untuk melanjutkan ke langkah berikutnya
            GameplayManager.Instance.NextStep();
        }
    }
}