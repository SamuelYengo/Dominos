using UnityEngine;
using TMPro;
using System.Collections.Generic;

public enum directions
{
    up,
    down,
    left,
    right
}

public class DominoScript : MonoBehaviour
{
    public GameObject side1;
    public GameObject side2;
    public TextMeshPro text1;
    public TextMeshPro text2;
    public GameManager gameManager;

    public float side1Value;
    public float side2Value;

    public float UpValue;
    public float DownValue;


    public float direction;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    // New Setup function called immediately after Instantiate
    public void Setup(float v1, float v2)
    {
        side1Value = v1;
        side2Value = v2;

        text1.text = "" + side1Value;
        text2.text = "" + side2Value;
    }

    void Update()
    {
        if (side1.transform.position.y > side2.transform.position.y) 
        {
            UpValue = side1Value;
            DownValue = side2Value;
        } else
        {
            UpValue = side2Value;
            DownValue = side1Value;
        }
    }

    void OnMouseDown()
    {
       // var playerScript = GetComponent<Player>();
        //if (playerScript.dominos.Contains(this.gameObject))
        //{
            Debug.Log("clicked: " + this);
            ClearFakeDoms();

            gameManager.CheckValidMoves(this);
        //}
        //else
       // {
        //Debug.Log("This domino is NOT in the player's hand. Cannot play it.");
        //}


    }
    public void ClearFakeDoms()
    {
        var FakeDoms = GameObject.FindGameObjectsWithTag("FakeDom");
        foreach (GameObject FakeDom in FakeDoms)
        {
            Destroy(FakeDom);
        }
    }

}