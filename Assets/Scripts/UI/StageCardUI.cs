using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StageCardUI : MonoBehaviour
{
    public Image icon;
    public Button button;
    public Image textImage;

    public void Init(ResepDataSO resep)
    {
        icon.sprite = resep.ikonResep;
        Debug.Log("Inisialisasi kartu stage: " + resep.namaResep);
        textImage.sprite = resep.resepText;

        button.onClick.AddListener(() =>
        {
            // Animasi efek klik
            transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 10, 1).OnComplete(() =>
            {
                // Simpan nama stage, dan Load scene gameplay'

                GameData.ResepDipilih = resep;
                SceneLoader.LoadScene("Story");
            });
        });
    }
}
