using UnityEngine;

public class CamZoom : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Camera cam;
    void Start(){
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll > 0f)
        {

        }
        else if (scroll < 0f)
        {

        }
    }
}
