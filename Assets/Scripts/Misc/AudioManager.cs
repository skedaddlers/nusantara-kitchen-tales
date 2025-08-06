using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioSource backgroundMusic;
    public AudioSource gameplayMusic;
    public AudioClip backgroundClip;
    public AudioClip gameplayClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (backgroundMusic == null)
        {
            backgroundMusic = gameObject.AddComponent<AudioSource>();
            backgroundMusic.clip = backgroundClip;
            backgroundMusic.loop = true;
        }

        if (gameplayMusic == null)
        {
            gameplayMusic = gameObject.AddComponent<AudioSource>();
            gameplayMusic.clip = gameplayClip;
            gameplayMusic.loop = true;
        }

        PlayMusic("background"); // Start with background music
    }

    public void PlayMusic(string musicType)
    {
        if (musicType == "background")
        {
            if (!backgroundMusic.isPlaying)
            {
                backgroundMusic.Play();
            }
            gameplayMusic.Stop();
        }
        else if (musicType == "gameplay")
        {
            if (!gameplayMusic.isPlaying)
            {
                gameplayMusic.Play();
            }
            backgroundMusic.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        backgroundMusic.volume = volume;
        gameplayMusic.volume = volume;
    }

    public float GetVolume()
    {
        return backgroundMusic.volume; // Assuming both music sources have the same volume
    }
    
}