using UnityEngine;
using TMPro;
using System.Collections.Generic;

public enum directions
{
    right,
    left,
    up,
    down,
}

public class DominoScript : MonoBehaviour
{
    public GameObject player;
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
    public void Setup(float v1, float v2, GameObject player )
    {
        side1Value = v1;
        side2Value = v2;

        text1.text = "" + side1Value;
        text2.text = "" + side2Value;
        this.player = player;
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
       //GameObject player = GetComponent<Player>();
       if (player.GetComponent<Player>().dominos.Contains(this.gameObject))
       {
            Debug.Log("clicked: " + this);
            ClearFakeDoms();

            gameManager.CheckValidMoves(this);
       }
       else
       {
        Debug.Log("This domino is not in the player's hand");
        } 


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