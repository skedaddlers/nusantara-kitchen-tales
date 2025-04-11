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
        // Tombol animasi masuk pakai DOTween
        playButton.transform.DOLocalMoveY(100f, 1f).From().SetEase(Ease.OutBack);
        customizeButton.transform.DOLocalMoveY(100f, 1f).From().SetDelay(0.2f).SetEase(Ease.OutBack);
        settingsButton.transform.DOLocalMoveY(0f, 1f).From().SetDelay(0.2f).SetEase(Ease.OutBack);
        quitButton.transform.DOLocalMoveY(-100f, 1f).From().SetDelay(0.4f).SetEase(Ease.OutBack);
        recipeButton.transform.DOLocalMoveY(-100f, 1f).From().SetDelay(0.4f).SetEase(Ease.OutBack);

        AddButtonEffect(playButton, PlayGame);
        AddButtonEffect(settingsButton, OpenSettings);
        AddButtonEffect(quitButton, QuitGame);
        AddButtonEffect(customizeButton, CustomizeCharacter);
        AddButtonEffect(recipeButton, OpenRecipeBook);
        
    }

    private void AddButtonEffect(Button btn, UnityAction action)
    {
        btn.onClick.AddListener(() =>
        {
            btn.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1)
                .OnComplete(() =>
                {
                    action?.Invoke();
                });
        });
    }

    public void CustomizeCharacter()
    {
        // Sementara tampilkan log
        Debug.Log("Customize character opened.");
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
        SceneLoader.LoadScene(SceneLoader.Scene.LevelSelect);

    }

    public void OpenSettings()
    {
        // Sementara tampilkan log
        Debug.Log("Settings opened.");
        // Bisa arahkan ke scene Settings nanti
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit pressed."); // Tidak akan quit di Editor, tapi jalan di Android
    }
}
