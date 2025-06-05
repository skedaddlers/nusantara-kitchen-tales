using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class StoryManager : MonoBehaviour
{
    public Image slideImage;
    public TextMeshProUGUI storyText;
    public Button nextButton;
    public Button homeButton;
    public StoryDataSO storyData;
    private int currentSlide = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        storyData = GameData.ResepDipilih.storyData;
        nextButton.onClick.AddListener(() =>
        {
            NextSlide();
        });
        ShowSlide(currentSlide);
        homeButton.onClick.AddListener(() =>
        {
            GameData.ResetData();
            SceneLoader.LoadScene("MainMenu");
        });
    }

    void ShowSlide(int index)
    {
        storyText.text = "";
        slideImage.sprite = storyData.slides[index].image;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(storyData.slides[index].text));
    }

    IEnumerator TypeText(string fullText)
    {
        storyText.text = "";
        foreach (char c in fullText)
        {
            storyText.text += c;
            yield return new WaitForSeconds(0.03f); // kecepatan ketik
        }
    }

    void NextSlide()
    {
        currentSlide++;
        if (currentSlide >= storyData.slides.Length)
        {
            // Cerita selesai, masuk ke gameplay
            SceneLoader.LoadScene("Gameplay"); // ganti sesuai scene kamu
        }
        else
        {
            ShowSlide(currentSlide);
        }
    }

    
}
