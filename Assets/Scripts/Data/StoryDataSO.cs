using UnityEngine;

[CreateAssetMenu(fileName = "StoryData", menuName = "NusantaraKitchen/Story Data")]
public class StoryDataSO : ScriptableObject
{
    [System.Serializable]
    public class Slide
    {
        public Sprite image;
        [TextArea] public string text;
    }

    public Slide[] slides;
}
