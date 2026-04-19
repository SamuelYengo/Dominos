using UnityEngine;

public class AntiZoom : MonoBehaviour
{
    private Camera mainCam;
    private float initialCamSize;
    private Vector3 initialLocalPos;

    [Header("Size Settings")]
    // Set this to the scale you want the object to ALWAYS be (e.g., 1, 1, 1)
    public Vector3 targetScale = Vector3.one;

    void Start()
    {
        mainCam = Camera.main;
        initialCamSize = mainCam.orthographicSize;

        // Record the buffer's starting position only
        initialLocalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        if (mainCam == null) return;

        float zoomFactor = mainCam.orthographicSize / initialCamSize;

        // 1. Force the scale to the Target Scale * zoomFactor
        // This ignores any changes made by other scripts or interactions
        transform.localScale = targetScale * zoomFactor;

        // 2. Counter-move the buffer
        transform.localPosition = initialLocalPos * zoomFactor;
    }
}