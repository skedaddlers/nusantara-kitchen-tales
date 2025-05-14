using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StageSelect : MonoBehaviour
{
    public PulauDatabase database;
    public Transform cardContainer;
    public GameObject stageCardPrefab;

    private void Start()
    {
        var pulauData = GameData.PulauDipilih;
        Debug.Log("Memuat Pulau: " + pulauData.namaPulau);
        if (pulauData == null)
        {
            Debug.LogError("Pulau tidak ditemukan di database.");
            return;
        }

        GenerateStageCards(pulauData);
    }

    void GenerateStageCards(PulauDataSO pulau)
    {
        for (int i = 0; i < pulau.resepList.Length; i++)
        {
            var card = Instantiate(stageCardPrefab, cardContainer);
            var ui = card.GetComponent<StageCardUI>();
            Debug.Log("Membuat kartu stage untuk: " + pulau.resepList[i].namaResep);
            ui.Init(pulau.resepList[i]);

            var rect = card.GetComponent<RectTransform>();
            var canvasGroup = card.GetComponent<CanvasGroup>();
            if (!canvasGroup) canvasGroup = card.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            rect.localScale = Vector3.one * 0.6f;

            // Animasi masuk: fade + scale-up
            canvasGroup.DOFade(1f, 0.4f).SetDelay(i * 0.05f);
            rect.DOScale(1f, 0.4f).SetDelay(i * 0.05f).SetEase(Ease.OutBack);
        }
        
    }
}