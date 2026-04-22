using UnityEngine;

public class AntiZoom : MonoBehaviour
{
    public Vector3 fixedWorldScale = Vector3.one;
    private Camera mainCam;
    private static float referenceCamSize = -1f;

    private Vector3 baseLocalPos; // The position at Zoom 1.0
    private bool isInitialized = false;

    void Awake()
    {
        mainCam = Camera.main;
        if (referenceCamSize < 0) referenceCamSize = mainCam.orthographicSize;
    }

    // Call this EVERY time the GameManager moves the domino in the hand
    public void SetBasePosition(Vector3 targetLocalPos)
    {
        float zoomFactor = mainCam.orthographicSize / referenceCamSize;
        // Divide by zoom to store the 'true' coordinate regardless of current zoom
        baseLocalPos = targetLocalPos / zoomFactor;
        isInitialized = true;
    }

    void LateUpdate()
    {
        if (transform.parent == null || !isInitialized) return;

        float zoomFactor = mainCam.orthographicSize / referenceCamSize;

        // Force scale to stay consistent
        Vector3 pScale = transform.parent.lossyScale;
        transform.localScale = new Vector3(
            (fixedWorldScale.x * zoomFactor) / pScale.x,
            (fixedWorldScale.y * zoomFactor) / pScale.y,
            (fixedWorldScale.z * zoomFactor) / pScale.z
        );

        // Force position to stay consistent
        transform.localPosition = baseLocalPos * zoomFactor;
    }
}