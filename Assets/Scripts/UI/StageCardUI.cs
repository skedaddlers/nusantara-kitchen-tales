using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StageCardUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI status;
    public Button button;

    public void Init(ResepDataSO resep, string namaResep, Sprite iconResep)
    {
        icon.sprite = iconResep;
        Debug.Log("Inisialisasi kartu stage: " + namaResep);
        title.text = namaResep;
        status.text = "Belum Dimulai";

        button.onClick.AddListener(() =>
        {
            // Animasi efek klik
            transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 10, 1).OnComplete(() =>
            {
                Debug.Log("Mulai masak: " + namaResep);
                // Simpan nama stage, dan Load scene gameplay'

                GameData.ResepDipilih = resep;
                SceneLoader.LoadScene("Story"); // Ganti dengan nama scene gameplay kamu
            });
        });
    }
}
