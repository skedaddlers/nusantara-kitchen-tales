using UnityEngine;

public static class ProgressManager
{
    // Kunci unik untuk setiap data: "NamaResep_Mode_Stars"
    // Contoh: "Nasi Pecel_Normal_Stars"
    private static string GetStarsKey(ResepDataSO resep, GameType mode)
    {
        // Gunakan resep.name karena itu unik untuk setiap asset ScriptableObject
        return resep.name + "_" + mode.ToString() + "_Stars";
    }

    /// <summary>
    /// Menyimpan jumlah bintang untuk sebuah resep dan mode.
    /// Hanya akan menyimpan jika jumlah bintang baru lebih tinggi dari yang lama.
    /// </summary>
    public static void SaveStars(ResepDataSO resep, GameType mode, int newStarCount)
    {
        // Mode Latihan tidak menyimpan progres
        if (mode == GameType.Practice) return;

        string key = GetStarsKey(resep, mode);
        int oldStarCount = LoadStars(resep, mode);

        // Hanya simpan jika skor baru lebih tinggi
        if (newStarCount > oldStarCount)
        {
            Debug.Log($"Menyimpan progres baru untuk {resep.name} mode {mode}: {newStarCount} bintang (sebelumnya {oldStarCount})");
            PlayerPrefs.SetInt(key, newStarCount);
            PlayerPrefs.Save(); // Memastikan data langsung ditulis ke disk
        }
    }

    /// <summary>
    /// Memuat jumlah bintang untuk sebuah resep dan mode.
    /// Mengembalikan 0 jika belum ada progres.
    /// </summary>
    public static int LoadStars(ResepDataSO resep, GameType mode)
    {
        string key = GetStarsKey(resep, mode);
        return PlayerPrefs.GetInt(key, 0); // Default ke 0 jika kunci tidak ada
    }

    /// <summary>
    /// (Opsional) Fungsi untuk mereset semua progres untuk testing.
    /// </summary>
    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Semua progres PlayerPrefs telah dihapus.");
    }
}