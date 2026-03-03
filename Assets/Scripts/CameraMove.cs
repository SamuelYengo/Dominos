using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector3 lastMousePosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Camera.main.ScreenToWorldPoint(Input.mousePosition)
                          - Camera.main.ScreenToWorldPoint(lastMousePosition);

            transform.position -= delta;
            lastMousePosition = Input.mousePosition;
        }
    }
}
