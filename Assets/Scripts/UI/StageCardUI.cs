using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StageCardUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI title;
    public Button button;

    public void Init(ResepDataSO resep)
    {
        icon.sprite = resep.ikonResep;
        Debug.Log("Inisialisasi kartu stage: " + resep.namaResep);
        title.text = resep.namaResep;

        button.onClick.AddListener(() =>
        {
            // Animasi efek klik
            transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 10, 1).OnComplete(() =>
            {
                Debug.Log("Mulai masak: " + title.text);
                // Simpan nama stage, dan Load scene gameplay'

                GameData.ResepDipilih = resep;
                SceneLoader.LoadScene("Story");
            });
        });
    }
}
