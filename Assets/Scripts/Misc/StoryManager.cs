using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class StoryManager : MonoBehaviour
{
    [System.Serializable]
    public class StorySlide
    {
        public Sprite image;
        [TextArea] public string text;
    }

    public Image slideImage;
    public TextMeshProUGUI storyText;
    public Button nextButton;

    public StorySlide[] slides;

    private int currentSlide = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        nextButton.onClick.AddListener(NextSlide);
        ShowSlide(currentSlide);
    }

    void ShowSlide(int index)
    {
        storyText.text = "";
        slideImage.sprite = slides[index].image;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(slides[index].text));
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
        if (currentSlide >= slides.Length)
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
