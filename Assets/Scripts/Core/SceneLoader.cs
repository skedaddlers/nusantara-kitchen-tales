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

    public static void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
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
