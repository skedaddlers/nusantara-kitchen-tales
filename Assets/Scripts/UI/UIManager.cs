using UnityEngine;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private void awake()
    {
        // Singleton pattern to ensure only one instance of UIManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

}