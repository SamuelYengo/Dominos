using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;



public class DominoScript : MonoBehaviour
{

    public enum Directions
{
    right,
    left,
    up,
    down,
}
    public GameObject player;
    public GameObject side1;
    public GameObject side2;
    public TextMeshPro text1;
    public TextMeshPro text2;
    public GameManager gameManager;

    public float side1Value;
    public float side2Value;



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
   
    }

    void OnMouseDown()
    {
      
       if (player.GetComponent<Player>().dominos.Contains(this.gameObject))
       {
            Debug.Log("dom clicked: " + this);
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

    public float getUpValue()
    {
        if (side1.transform.position.y > side2.transform.position.y) {
            return side1Value;
        } else{
            return side2Value;
        }
    }

    public float getDownValue()
    {
        if (side1.transform.position.y > side2.transform.position.y)
        {
            return side2Value;
        }
        else
        {
            return side1Value;
        }
    }
    
    public override string ToString()
    {
        return $"{side1Value} | {side2Value}";
    }
}