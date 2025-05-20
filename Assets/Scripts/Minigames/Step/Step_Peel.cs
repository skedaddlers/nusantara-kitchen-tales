using UnityEngine;

// For peeling potatoes or other vegetables
[System.Serializable]
public class Step_Peel : Step
{
    public RenderTexture renderTexture;
    public Material material;
    public Texture2D brushTexture;
    public RectTransform targetUI;
    public Camera camera;

    private bool isDragging = false;

    void Start()
    {
        camera = Camera.main;
    }

    void OnEnable()
    {
        InputHandler.Instance.OnTouchStarted += StartDrag;
        InputHandler.Instance.OnTouchMoved += ContinueDrag;
        InputHandler.Instance.OnTouchCanceled += EndDrag;
    }

    void OnDisable()
    {
        if (InputHandler.Instance == null) return;
        InputHandler.Instance.OnTouchStarted -= StartDrag;
        InputHandler.Instance.OnTouchMoved -= ContinueDrag;
        InputHandler.Instance.OnTouchCanceled -= EndDrag;
    }

    private void StartDrag(Vector2 position)
    {
        isDragging = true;
    }

    private void EndDrag(Vector2 position)
    {
        isDragging = false;
    }

    private void ContinueDrag(Vector2 pos)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetUI, pos, camera, out localPos);
        Vector2 uvPos = RectTransformToUV(targetUI, localPos);
        DrawAtUV(uvPos);
    }

    Vector2 RectTransformToUV(RectTransform rectTransform, Vector2 localPos)
    {
        Vector2 pivot = rectTransform.pivot;
        Vector2 size = rectTransform.rect.size;
        Vector2 uv = new Vector2((localPos.x + pivot.x * size.x) / size.x, (localPos.y + pivot.y * size.y) / size.y);
        return uv;
    }

    void DrawAtUV(Vector2 uv)
    {
        if (!isDragging) return;

        RenderTexture.active = renderTexture;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, renderTexture.width, renderTexture.height, 0);

        Debug.Log("Drawing at UV: " + uv);

        
        float x = uv.x * renderTexture.width - brushTexture.width * 0.5f;
        float y = (1.0f - uv.y) * renderTexture.height - brushTexture.height * 0.5f;

        Graphics.DrawTexture(new Rect(x, y, brushTexture.width, brushTexture.height), brushTexture, material);
        Debug.Log("Drawing at: " + new Rect(x, y, brushTexture.width, brushTexture.height));    
        GL.PopMatrix();


        RenderTexture.active = null;
    }

}   