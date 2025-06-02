using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CustomizationManager : MonoBehaviour
{
    public Image characterImage;
    public Image skinDisplayImage;
    public TMP_Text characterNameText;
    public Sprite[] characterSprites;
    public string[] characterNames;
    public Button nextButton;
    public Button previousButton;
    public Button confirmButton;
    public Button homeButton;
    public int currentCharacterIndex = 0;


    private void Start()
    {
        UpdateCharacterDisplay();
        nextButton.onClick.AddListener(() => { AnimateButton(nextButton); NextCharacter(); });
        previousButton.onClick.AddListener(() => { AnimateButton(previousButton); PreviousCharacter(); });
        confirmButton.onClick.AddListener(() => { AnimateConfirmButton(confirmButton); ConfirmSelection(); });
        homeButton.onClick.AddListener(() => { AnimateButton(homeButton); SceneLoader.LoadScene("MainMenu"); });
    }

    private void UpdateCharacterDisplay()
    {
        if (currentCharacterIndex < 0 || currentCharacterIndex >= characterSprites.Length)
            return;

        // Animate skin image fade out and scale down before changing
        skinDisplayImage.DOFade(0, 0.2f).OnComplete(() =>
        {
            skinDisplayImage.sprite = characterSprites[currentCharacterIndex];
            // skinDisplayImage.SetNativeSize();
            skinDisplayImage.DOFade(1, 0.2f);
            skinDisplayImage.transform.DOScale(1.05f, 0.2f).From(0.9f);
        });

        characterNameText.DOFade(0, 0.2f).OnComplete(() =>
        {
            characterNameText.text = characterNames[currentCharacterIndex];
            characterNameText.DOFade(1, 0.2f);
            characterNameText.transform.DOScale(1.0f, 0.2f).From(0.9f).SetEase(Ease.OutBack);
        });
    }

    private void NextCharacter()
    {
        currentCharacterIndex++;
        if (currentCharacterIndex >= characterSprites.Length)
            currentCharacterIndex = 0;
        UpdateCharacterDisplay();
    }

    private void PreviousCharacter()
    {
        currentCharacterIndex--;
        if (currentCharacterIndex < 0)
            currentCharacterIndex = characterSprites.Length - 1;
        UpdateCharacterDisplay();
    }

    private void ConfirmSelection()
    {
        Debug.Log("Character confirmed: " + characterNames[currentCharacterIndex]);

        // Animate character image
        characterImage.DOFade(0, 0.2f).OnComplete(() =>
        {
            characterImage.sprite = characterSprites[currentCharacterIndex];
            // characterImage.SetNativeSize();
            characterImage.DOFade(1, 0.2f);
            characterImage.transform.DOScale(1.0f, 0.3f).From(0.8f).SetEase(Ease.OutBack);
        });
    }

    private void AnimateButton(Button button)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.DOKill(); // Stop any ongoing animation
        rect.DOScale(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo);
    }

    private void AnimateConfirmButton(Button button)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.Append(rect.DOScale(1.2f, 0.1f));
        seq.Append(rect.DOScale(1.0f, 0.1f));
        seq.Join(button.image.DOColor(Color.green, 0.15f).SetLoops(2, LoopType.Yoyo));
    }
}
