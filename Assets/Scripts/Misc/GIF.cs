using UnityEngine;
using UnityEngine.UI;

public class GIF : MonoBehaviour
{
    public Sprite[] gifFrames;
    public float frameRate = 1f; // Time in seconds between frames
    private Image imageComponent;
    private int currentFrame = 0;
    private float timer = 0f;
    private bool isPlaying = false;
    private void Awake()
    {
        
        if (imageComponent == null)
        {
            Debug.LogError("GIF component requires an Image component.");
        }


    }

    private void Start()
    {
        imageComponent = GetComponent<Image>();
        if (gifFrames.Length > 0)
        {
            imageComponent.sprite = gifFrames[0];
        }
    }

    private void Update()
    {
        if (isPlaying && gifFrames.Length > 0)
        {
            timer += Time.deltaTime;
            if (timer >= frameRate)
            {
                timer = 0f;
                currentFrame = (currentFrame + 1) % gifFrames.Length;
                imageComponent.sprite = gifFrames[currentFrame];
            }
        }
    }

    public void Play()
    {
        if (gifFrames.Length == 0)
        {
            Debug.LogError("No frames to play in GIF.");
            return;
        }
        isPlaying = true;
        currentFrame = 0;
        timer = 0f;
        imageComponent.sprite = gifFrames[currentFrame];
    }
}