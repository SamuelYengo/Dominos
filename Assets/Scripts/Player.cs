using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public List<GameObject> dominos = new List<GameObject>();
    public int Teams;
    public GameManager gameManager;

    void Start()
    {
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
            }
        }
    }

}