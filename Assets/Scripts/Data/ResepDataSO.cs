using UnityEngine;

[CreateAssetMenu(fileName = "ResepData", menuName = "NusantaraKitchen/Resep")]
public class ResepDataSO : ScriptableObject
{
    public string namaResep;
    public Sprite ikonResep;
    public StoryDataSO storyData;

    // Placeholder: kamu bisa tambah lagi nanti
    public int estimasiDurasi;
    public string[] bahan;
}
