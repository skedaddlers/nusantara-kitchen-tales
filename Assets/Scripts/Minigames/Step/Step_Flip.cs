using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class Step_Flip : Step
{
    public Image bahanImage;
    public Bahan bahan;
    public float minVerticalSwipeDistance = 100f;

    private Vector2 startTouchPosition;
    private bool isFlipped = false;

    private void Awake()
    {
        int currentStep = GameplayManager.Instance.CurrentStep;
        bahan = GameData.ResepDipilih.langkahMasak[currentStep].bahanDiperlukan[0];
        bahanImage.sprite = bahan.gambarBahan;

    }

    void OnEnable()
    {
        InputHandler.Instance.OnTouchStarted += StartFlip;
        InputHandler.Instance.OnTouchMoved += ContinueFlip;
        InputHandler.Instance.OnTouchCanceled += EndFlip;
    }

    void OnDisable()
    {
        if (InputHandler.Instance == null) return;
        InputHandler.Instance.OnTouchStarted -= StartFlip;
        InputHandler.Instance.OnTouchMoved -= ContinueFlip;
        InputHandler.Instance.OnTouchCanceled -= EndFlip;
    }

    private void StartFlip(Vector2 position)
    {
        startTouchPosition = position;
    }

    private void ContinueFlip(Vector2 position)
    {
        if (isFlipped) return;

        float verticalDistance = position.y - startTouchPosition.y;
        

        if (Mathf.Abs(verticalDistance) > minVerticalSwipeDistance)
        {
            Flip();
        }
    }

    private void EndFlip(Vector2 position)
    {
        // Reset touch position
        startTouchPosition = Vector2.zero;
    }

    private void Flip()
    {
        isFlipped = true;

        // Ganti gambar bahan
        if (bahanImage != null)
        {
            bahanImage.sprite = bahan.gambarBahanTerbalik;


            // Tambahkan efek flip dotween
            bahanImage.transform.DORotate(new Vector3(180, 0, 0), 0.5f).SetEase(Ease.OutBack);


            bahanImage.rectTransform.localScale = new Vector3(-1, 1, 1);

        }
        else
        {
            Debug.LogError("Bahan Image tidak di-set!");
        }

        Invoke("NextStep", 1f);
    }

    private void NextStep()
    {
        GameplayManager.Instance.NextStep();
    }




}