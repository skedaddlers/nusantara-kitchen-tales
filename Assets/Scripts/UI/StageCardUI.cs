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
        Utilz.SetSizeNormalized(textImage.GetComponent<RectTransform>(), resep.resepText, 350f, 350f);

        button.onClick.AddListener(() =>
        {
            // Animasi efek klik
            transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 10, 1).OnComplete(() =>
            {
                // Simpan nama stage, dan Load scene gameplay'

                GameData.ResepDipilih = resep;
                // get stage select component
                var stageSelect = FindObjectOfType<StageSelect>();
                if (stageSelect == null)
                {
                    Debug.LogError("StageSelect component not found in the scene.");
                    return;
                }
                stageSelect.ShowPanel(resep);
            });
        });
    }
}
