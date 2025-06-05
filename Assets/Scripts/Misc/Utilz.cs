
using UnityEngine;
using UnityEngine.UI;
public class Utilz
{
    public static void SetSizeNormalized(RectTransform rectTransform, Sprite sprite, float MAX_WIDTH = 300f, float MAX_HEIGHT = 300f)
    {
        if (rectTransform == null || sprite == null) return;

        float width = sprite.rect.width;
        float height = sprite.rect.height;

        // Normalize the size to a maximum of 150x150
        float scaleFactor = Mathf.Min(MAX_WIDTH / width, MAX_HEIGHT / height);
        rectTransform.sizeDelta = new Vector2(width * scaleFactor, height * scaleFactor);
    }
}