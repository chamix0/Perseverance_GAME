using UnityEngine;

public class CursorManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private bool hideOnStart = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        if (hideOnStart)
        {
            Cursor.visible = false;
        }
    }

    public static void HideCursor()
    {
        if (Cursor.visible)
        {
            Cursor.visible = false;
        }
    }

    public static void ShowCursor()
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}