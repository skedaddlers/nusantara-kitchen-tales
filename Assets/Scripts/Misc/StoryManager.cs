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
            AudioManager.Instance.PlayMusic("background");
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
            if(GameData.GameType == GameType.Practice)
            {
                SceneLoader.LoadScene("Latihan");
            }
            else if (GameData.GameType == GameType.Normal)
            {
                SceneLoader.LoadScene("Gameplay");
            }
            else if (GameData.GameType == GameType.SkillTest)
            {
                SceneLoader.LoadScene("SkillTest");
            }
        }
        else
        {
            ShowSlide(currentSlide);
        }
    }

    
}
