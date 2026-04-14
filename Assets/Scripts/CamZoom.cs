using UnityEngine;

public class CamZoom : MonoBehaviour
{
    private Camera cam;

    private float zoomSpeed = 2f;
    private float minSize = 2f;
    private float maxSize = 10f;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam.orthographicSize == 0) cam.orthographicSize = 5f;
    }
    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            float newSize = cam.orthographicSize - (scroll * zoomSpeed);
            cam.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);
        }
    }
}