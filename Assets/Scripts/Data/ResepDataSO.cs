using UnityEngine;
using System.Collections.Generic;



[CreateAssetMenu(fileName = "ResepData", menuName = "NusantaraKitchen/Resep")]
public class ResepDataSO : ScriptableObject
{
    public string namaResep;
    public Sprite ikonResep;
    public StoryDataSO storyData;
    public Sprite resepText;

    // Placeholder: kamu bisa tambah lagi nanti
    public int estimasiDurasi;
    public LangkahMasak[] langkahMasak;
    
    [System.Serializable]
    public class LangkahMasak
    {
        public string deskripsi;

        [SerializeReference]
        public Step step;
        
        public StepType jenisStep;
        public Sprite ikonStep;
        public Bahan[] bahanDiperlukan;
        public Alat[] alatDiperlukan;
        public GameObject prefabStep;
        public bool addToExisting = false; // untuk menambah alat ke existing
    }

    public enum StepType
    {
        DragDrop,
        ButtonPress,
        Slider,
        Tap,
        MeterControl,
        Spin,
        Peel,
        Flip,
        Pound,
        Cut,
        // Tambahkan jenis langkah lainnya jika perlu
    }
}
