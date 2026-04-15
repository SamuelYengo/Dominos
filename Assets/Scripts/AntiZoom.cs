using UnityEngine;

public class AntiZoom : MonoBehaviour
{
    private Camera mainCam;
    public float initialCamSize;
    public Vector3 initialLocalPos;
    private Vector3 initialScale;

    void Start()
    {
        mainCam = Camera.main;
        initialCamSize = mainCam.orthographicSize;
        initialLocalPos = transform.localPosition;
        initialScale = transform.localScale;
    }

    void LateUpdate() // Use LateUpdate to ensure it happens after Camera Zoom
    {
        if (mainCam == null) return;

        // Calculate how much the camera has scaled
        float zoomFactor = mainCam.orthographicSize / initialCamSize;

        // 1. Maintain visual size
        transform.localScale = initialScale * zoomFactor;

        // 2. Maintain visual position relative to the camera frame
        // This stops them from sliding toward the center
        //transform.localPosition = initialLocalPos * zoomFactor;
    }
}