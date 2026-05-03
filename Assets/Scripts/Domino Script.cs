using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DominoScript : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public GameObject side1; // The GameObject representing the first half
    public GameObject side2; // The GameObject representing the second half
    public TextMeshPro text1;
    public TextMeshPro text2;
    public GameManager gameManager;

    [Header("Data")]
    public int side1Value;
    public int side2Value;

    private AudioSource clickSound;

    void Awake()
    {
        // Automatically find the GameManager in the scene
        gameManager = FindFirstObjectByType<GameManager>();
        clickSound = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Called by GameManager to initialize the domino's values and owner.
    /// </summary>
    public void Setup(int v1, int v2, GameObject playerOwner)
    {
        side1Value = v1;
        side2Value = v2;

        if (text1 != null) text1.text = side1Value.ToString();
        if (text2 != null) text2.text = side2Value.ToString();

        player = playerOwner;
    }

    void OnMouseDown()
    {
        // Only allow interaction if it's currently in a player's hand
        if (player != null && player.GetComponent<Player>().dominos.Contains(this.gameObject))
        {
            Debug.Log("Domino clicked: " + this.ToString());
            ClearFakeDoms();
            gameManager.CheckValidMoves(this);
            clickSound.Play();
        }
        else
        {
            Debug.Log("This domino is not in the player's hand or already played.");
        }
    }

    public void ClearFakeDoms()
    {
        GameObject[] fakeDoms = GameObject.FindGameObjectsWithTag("FakeDom");
        foreach (GameObject fake in fakeDoms)
        {
            Destroy(fake);
        }
    }

    /// <summary>
    /// Uses world position to determine which side of the domino is facing the "open" end of the board.
    /// </summary>
    public int GetOpenValue(Directions boardDir)
    {
        // If it's a double, both sides are the same
        if (side1Value == side2Value) return side1Value;

        if (boardDir == Directions.right || boardDir == Directions.left)
        {
            if (side1.transform.position.x > side2.transform.position.x)
                return (boardDir == Directions.right) ? side1Value : side2Value;
            else
                return (boardDir == Directions.right) ? side2Value : side1Value;
        }

        if (boardDir == Directions.up || boardDir == Directions.down)
        {
            if (side1.transform.position.y > side2.transform.position.y)
                return (boardDir == Directions.up) ? side1Value : side2Value;
            else
                return (boardDir == Directions.up) ? side2Value : side1Value;
        }

        return -1;
    }

    /// <summary>
    /// Highlights only the side of the domino that contributes to the board score.
    /// </summary>
    public void HighlightScoringSide(bool active, Color highlightColor, Directions boardDir)
    {
        // 1. Reset both sides to white (default)
        SetObjectColor(side1, Color.white);
        SetObjectColor(side2, Color.white);

        if (!active) return;

        // 2. If it's a double, the whole tile counts toward the score
        if (side1Value == side2Value)
        {
            SetObjectColor(side1, highlightColor);
            SetObjectColor(side2, highlightColor);
            return;
        }

        // 3. Find the "outer" GameObject based on the board direction
        GameObject outerSide = null;

        if (boardDir == Directions.right || boardDir == Directions.left)
        {
            if (side1.transform.position.x > side2.transform.position.x)
                outerSide = (boardDir == Directions.right) ? side1 : side2;
            else
                outerSide = (boardDir == Directions.right) ? side2 : side1;
        }
        else if (boardDir == Directions.up || boardDir == Directions.down)
        {
            if (side1.transform.position.y > side2.transform.position.y)
                outerSide = (boardDir == Directions.up) ? side1 : side2;
            else
                outerSide = (boardDir == Directions.up) ? side2 : side1;
        }

        // 4. Apply the color to the specific side found
        if (outerSide != null)
        {
            SetObjectColor(outerSide, highlightColor);
        }
    }

    private void SetObjectColor(GameObject obj, Color c)
    {
        if (obj == null) return;

        // Change color of the side renderer and any pips/children
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var r in renderers)
        {
            r.color = c;
        }
    }

    public override string ToString()
    {
        return $"{side1Value} | {side2Value}";
    }
}