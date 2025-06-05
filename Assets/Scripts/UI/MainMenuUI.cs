using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;


public class MainMenuUI : MonoBehaviour
{
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;
    public Button customizeButton;
    public Button recipeButton;

    void Start()
    {
        playButton = GameObject.Find("Play").GetComponent<Button>();
        settingsButton = GameObject.Find("Settings").GetComponent<Button>();
        quitButton = GameObject.Find("Quit").GetComponent<Button>();
        customizeButton = GameObject.Find("Kostum").GetComponent<Button>();
        recipeButton = GameObject.Find("Resep").GetComponent<Button>();
        Debug.Log("MainMenuUI started.");
        Animate();
    }

    public void Reload()
    {
        Debug.Log("Reloading MainMenuUI");
        Animate();
    }

    private void Animate()
    {
        if (playButton == null || settingsButton == null || quitButton == null || customizeButton == null || recipeButton == null)
        {
            Debug.LogError("One or more buttons are not assigned in the MainMenuUI script.");
            return;
        }

        Debug.Log("Animating MainMenuUI buttons.");

        AddButtonEffect(playButton, PlayGame);
        AddButtonEffect(settingsButton, OpenSettings);
        AddButtonEffect(quitButton, QuitGame);
        AddButtonEffect(customizeButton, CustomizeCharacter);
        AddButtonEffect(recipeButton, OpenRecipeBook);

        // Kill previous tweens (important for reloading)
        DOTween.Kill(playButton.transform);
        DOTween.Kill(settingsButton.transform);
        DOTween.Kill(quitButton.transform);
        DOTween.Kill(customizeButton.transform);
        DOTween.Kill(recipeButton.transform);

        // Optional: Reset scale and position manually if needed
        playButton.transform.localScale = Vector3.one;
        settingsButton.transform.localScale = Vector3.one;
        quitButton.transform.localScale = Vector3.one;
        customizeButton.transform.localScale = Vector3.one;
        recipeButton.transform.localScale = Vector3.one;

        // Re-animate positions
        playButton.transform.DOLocalMoveY(100f, 1f).From().SetEase(Ease.OutBack);
        customizeButton.transform.DOLocalMoveY(100f, 1f).From().SetDelay(0.2f).SetEase(Ease.OutBack);
        settingsButton.transform.DOLocalMoveY(0f, 1f).From().SetDelay(0.2f).SetEase(Ease.OutBack);
        quitButton.transform.DOLocalMoveY(-100f, 1f).From().SetDelay(0.4f).SetEase(Ease.OutBack);
        recipeButton.transform.DOLocalMoveY(-100f, 1f).From().SetDelay(0.4f).SetEase(Ease.OutBack);
    }


    private void AddButtonEffect(Button btn, UnityAction action)
    {
        btn.onClick.RemoveAllListeners();

        Debug.Log("Adding click effect to button: " + btn.name);
        btn.onClick.AddListener(() =>
        {
            Debug.Log("Button clicked — skipping tween");
            action?.Invoke();
        });
    }


    public void CustomizeCharacter()
    {
        // Sementara tampilkan log
        Debug.Log("Customize character opened.");
        SceneLoader.LoadScene("CUstomization");
        // Bisa arahkan ke scene CustomizeCharacter nanti
    }

    public void OpenRecipeBook()
    {
        // Sementara tampilkan log
        Debug.Log("Recipe book opened.");
        // Bisa arahkan ke scene RecipeBook nanti
    }

    public void PlayGame()
    {
        // Ganti ke scene gameplay
        Debug.Log("PlayPressed");
        SceneLoader.LoadScene("PulauSelect");

    }

    public void OpenSettings()
    {
        // Sementara tampilkan log
        Debug.Log("Settings opened.");
        SceneLoader.LoadScene("Settings");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit pressed."); // Tidak akan quit di Editor, tapi jalan di Android
    }
}
