using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        MainMenu,
        LevelSelect,
        Settings
    }

    public static void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public static void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game triggered.");
    }
}
