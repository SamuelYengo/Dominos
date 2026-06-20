using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public List<GameObject> dominos = new List<GameObject>();
    public int Teams;
    public GameManager gameManager;

    private Camera cam;
    private Vector3 initialLocalPos;
    private float defaultCamSize = 5f;
    void Start()
    {
        cam = Camera.main;
        initialLocalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        if (cam == null) return;


        float ratio = cam.orthographicSize / defaultCamSize;
        transform.localPosition = initialLocalPos * ratio;

    }
    void Update()
    {
    }

    public void hideDominos()
    {
        foreach (var domino in dominos)
        {
            if (domino != null)
            {
                domino.SetActive(false);
            }
        }
    }

    public void showDominos()
    {
        foreach (var domino in dominos)
        {
            if (domino != null)
            {
                domino.SetActive(true);
                DominoScript script = domino.GetComponent<DominoScript>();
                if (script != null)
                {
                    script.RefreshText();
                }
            }
        }
    }

}
