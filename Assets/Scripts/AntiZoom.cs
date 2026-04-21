using UnityEngine;

public class AntiZoom : MonoBehaviour
{
    [Header("Global Settings")]
    public Vector3 fixedWorldScale = Vector3.one;

    private Camera mainCam;
    private static float referenceCamSize = -1f;

    // This stores the 'Visual Distance' at the moment of placement
    private Vector3 initialScreenOffset;

    void Start()
    {
        mainCam = Camera.main;

        if (referenceCamSize < 0)
        {
            referenceCamSize = mainCam.orthographicSize;
        }

        // 1. Calculate the distance in World Space
        Vector3 worldOffset = transform.position - transform.parent.position;

        // 2. Normalize it: Divide by current zoom so we know the 'base' distance
        float currentZoomRatio = mainCam.orthographicSize / referenceCamSize;
        initialScreenOffset = worldOffset / currentZoomRatio;

        MaintainScaleAndPos();
    }

    void LateUpdate()
    {
        MaintainScaleAndPos();
    }

    void MaintainScaleAndPos()
    {
        if (mainCam == null || transform.parent == null) return;

        float zoomFactor = mainCam.orthographicSize / referenceCamSize;

        // --- SCALE ---
        // Lock the scale so it never looks bigger or smaller on screen
        Vector3 pScale = transform.parent.lossyScale;
        transform.localScale = new Vector3(
            (fixedWorldScale.x * zoomFactor) / pScale.x,
            (fixedWorldScale.y * zoomFactor) / pScale.y,
            (fixedWorldScale.z * zoomFactor) / pScale.z
        );

        // --- POSITION ---
        // This is the magic line. We force the domino to stay at the 
        // original offset, scaled by the zoom, relative to the player.
        // This prevents them from 'drifting' or 'squishing' during live zoom.
        transform.position = transform.parent.position + (initialScreenOffset * zoomFactor);
    }
}