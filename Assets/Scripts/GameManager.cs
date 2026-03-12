using UnityEngine;
using System;
using System.Numerics;
using System.Collections.Generic;
using Directions = DominoScript.Directions;

public class GameManager : MonoBehaviour
{
    public List<System.Numerics.Vector2> dominoVectorsList;

    public GameObject DominoObject;
    public GameObject FakeObject;
    public GameObject[] Players;

    public int Team1Score;
    public int Team2Score;
    public UnityEngine.Vector3 offset;
    public int playerWhosTurnItIsIndex;
    public GameObject RootDomino;
    public GameObject[] Ends;
    string lastEndsState = "";


    void Start()
    {
        PreRound();
        RoundStart();
        Display();
    }

    void Update()
    {
        if (Team1Score >= 610 || Team2Score >= 610)
        {
            if (isRoundEnded())
            {
                RoundEnd();
            }
        }
        else
        {
            EndGame();
        }

        PrintEndsIfChanged();
    }
   
    public float GetBoardEndTotal()
    {
        if (RootDomino == null)
            return 0;

        float total = 0;

        // Count how many ends still point to the root domino
        int rootOccurrences = 0;
        for (int i = 0; i < Ends.Length; i++)
        {
            if (Ends[i] == RootDomino)
                rootOccurrences++;
        }

        var root = RootDomino.GetComponent<DominoScript>();

        // Root domino scoring
        if (root.side1Value == root.side2Value) // it's a double
        {
            if (rootOccurrences >= 3)
            {
                // root still has open sides
                total += root.side1Value + root.side2Value;
            }
            // if exactly 2 connections → closed → add nothing
        }
        else
        {
            total += root.side1Value + root.side2Value;
        }

        // Check each direction individually
        if (Ends[0] != null && Ends[0] != RootDomino)
            total += GetEndValue(Ends[0], Directions.right);

        if (Ends[1] != null && Ends[1] != RootDomino)
            total += GetEndValue(Ends[1], Directions.left);

        if (Ends[2] != null && Ends[2] != RootDomino)
            total += GetEndValue(Ends[2], Directions.up);

        if (Ends[3] != null && Ends[3] != RootDomino)
            total += GetEndValue(Ends[3], Directions.down);

        return total;
    }
    public float GetEndValue(GameObject endObj, Directions dir)
    {
        if (endObj == null) return 0;

        var end = endObj.GetComponent<DominoScript>();
        //int dir = Mathf.RoundToInt(end.direction);

        switch (dir)
        {
            case Directions.right:
                return rightDirCases(endObj);
            case Directions.left:
                return leftDirCases(endObj);
            case Directions.down:
                return downDirCases(endObj);
            case Directions.up:
                return upDirCases(endObj);
            default:
                Debug.LogError("Invalid direction");
                return 0;

        }

    }

    float rightDirCases(GameObject endObj)
    {
        var end = endObj.GetComponent<DominoScript>();
        int dir = Mathf.RoundToInt(end.direction);

        switch (dir)
        {
            case 0:
            case 90:
            case 270:
                return end.getUpValue();

            case 180:
                return end.getDownValue();

            default:
                Debug.LogError("Invalid direction");
                return -1;
        }
    }

    float leftDirCases(GameObject endObj)
    {
        var end = endObj.GetComponent<DominoScript>();
        int dir = Mathf.RoundToInt(end.direction);

        switch (dir)
        {
            case 0:
            case 90:
            case 270:
                return end.getDownValue();

            case 180:
                return end.getUpValue();

            default:
                Debug.LogError("Invalid direction");
                return -1;
        }
    }
    float upDirCases(GameObject endObj)
    {
        var end = endObj.GetComponent<DominoScript>();
        int dir = Mathf.RoundToInt(end.direction);

        switch (dir)
        {
            case 0:
            case 180:
            case 270:
                return end.getUpValue();
            case 90:
                return end.getDownValue();

            default:
                Debug.LogError("Invalid direction");
                return -1;
        }
    }
    float downDirCases(GameObject endObj)
    {
        var end = endObj.GetComponent<DominoScript>();
        int dir = Mathf.RoundToInt(end.direction);

        switch (dir)
        {
            case 0:
            case 180:
            case 90:
                return end.getUpValue();

            case 270:
                return end.getDownValue();

            default:
                Debug.LogError("Invalid direction");
                return -1;
        }
    }

    void PrintEndsIfChanged()
    {
string currentState =
    $"up: {Ends[2].GetComponent<DominoScript>().getUpValue()} | {Ends[2].GetComponent<DominoScript>().getDownValue()}, " +
    $"down: {Ends[3].GetComponent<DominoScript>().getUpValue()} | {Ends[3].GetComponent<DominoScript>().getDownValue()}, " +
    $"left: {Ends[1].GetComponent<DominoScript>().getUpValue()} | {Ends[1].GetComponent<DominoScript>().getDownValue()}, " +
    $"right: {Ends[0].GetComponent<DominoScript>().getUpValue()} | {Ends[0].GetComponent<DominoScript>().getDownValue()}";
        if (currentState != lastEndsState)
        {
            lastEndsState = currentState;
            Debug.Log(currentState);
        }
    }

    void RoundEnd() { }

    bool isRoundEnded()
    {
        return false;
    }

    void EndGame() { }

    public void nextPlayerTurn()
    {
        playerWhosTurnItIsIndex += 1;
        if (playerWhosTurnItIsIndex >= Players.Length)
        {
            playerWhosTurnItIsIndex = 0;
        }

        Display();
      
    }

    void Display()
    {
        foreach (var player in Players) { player.GetComponent<Player>().hideDominos(); }
        Players[playerWhosTurnItIsIndex].GetComponent<Player>().showDominos();

        if (RootDomino != null)
        {
            RootDomino.SetActive(true);
            RootDomino.transform.position = new UnityEngine.Vector3(0, 0, 0);
        }

    }

    void PreRound()
    {
        dominoVectorsList = new List<System.Numerics.Vector2>();

        for (int i = 0; i <= 6; i++)
        {
            for (int j = i; j <= 6; j++)
            {
                dominoVectorsList.Add(new System.Numerics.Vector2((float)i, (float)j));
            }
        }

        // set ends to directions
   
    }

    public void CreateDominos(GameObject player, int howManyDominos)
    {
        for (int i = 0; i < howManyDominos; i++)
        {
            if (dominoVectorsList.Count == 0) break;
            UnityEngine.Vector3 dominoPosition = player.transform.position + offset * i;
            GameObject newDomino = Instantiate(DominoObject, dominoPosition, transform.rotation);
            newDomino.transform.SetParent(player.transform, true);
            player.GetComponent<Player>().dominos.Add(newDomino);
            var dominoScript = newDomino.GetComponent<DominoScript>();
            int randomIndex = UnityEngine.Random.Range(0, dominoVectorsList.Count);
            System.Numerics.Vector2 randomDomino = dominoVectorsList[randomIndex];
            dominoVectorsList.RemoveAt(randomIndex);
            dominoScript.Setup(randomDomino.X, randomDomino.Y, player);
        }
    }

    public void RoundStart()
    {
        // Setup all players
        CreateDominos(Players[0], 7);
        CreateDominos(Players[1], 7);
        CreateDominos(Players[2], 7);
        CreateDominos(Players[3], 7);

        bool rootFound = false;

        for (int i = 0; i < Players.Length; i++)
        {
            var player = Players[i].GetComponent<Player>();

            for (int j = player.dominos.Count - 1; j >= 0; j--)
            {
                var domino = player.dominos[j];
                var cur = domino.GetComponent<DominoScript>();

                float v1 = cur.side1Value;
                float v2 = cur.side2Value;

                bool is_double = v1 == v2;
                if (is_double) // It's a double
                {
                    player.dominos.RemoveAt(j);
                    RootDomino = domino;
                    RootDomino.transform.SetParent(null, false);

                    //point ends to domino
                    Ends[0] = RootDomino;
                    Ends[1] = RootDomino;
                    Ends[2] = RootDomino;
                    Ends[3] = RootDomino;


                    
                    playerWhosTurnItIsIndex = i;
                    rootFound = true;



                    break;
                }
            }

            if (rootFound)
                break;
        }

        if (!rootFound)
        {
            Debug.LogError("NO ROOT DOMINO FOUND — all dominos checked");
        }

    }

    public void CheckValidMoves(DominoScript domino)
    {

        float v1 = domino.side1Value;
        float v2 = domino.side2Value;


        var rightEnd = Ends[0].GetComponent<DominoScript>();
        int Rdir = Mathf.RoundToInt(rightEnd.direction);
        float rval = (Rdir == 180) ? rightEnd.side1Value : rightEnd.side2Value;
        makeFakeDom(v1, v2, rightEnd, 0, rval, new UnityEngine.Vector3(2f, 0f, 0f), domino);

        var LeftEnd = Ends[1].GetComponent<DominoScript>();
        int Ldir = Mathf.RoundToInt(LeftEnd.direction);
        float lval = (Ldir == 180) ? LeftEnd.side2Value : LeftEnd.side1Value;
        makeFakeDom(v1, v2, LeftEnd, 1, lval, new UnityEngine.Vector3(-2f, 0f, 0f), domino);

        var UpEnd = Ends[2].GetComponent<DominoScript>();
        int Udir = Mathf.RoundToInt(UpEnd.direction);
        float Uval = (Udir == 90) ? UpEnd.side1Value : UpEnd.side2Value;
        makeFakeDom(v1, v2, UpEnd, 2, Uval, new UnityEngine.Vector3(0f, 2f, 0f), domino);

        var DownEnd = Ends[3].GetComponent<DominoScript>();
        int Ddir = Mathf.RoundToInt(DownEnd.direction);
        float Dval = (Ddir == 270) ? DownEnd.side1Value : DownEnd.side2Value;
        makeFakeDom(v1, v2, DownEnd, 3, Dval, new UnityEngine.Vector3(0f, -2f, 0f), domino);
    }

    public float getOffset(DominoScript end)
    {
        int dir = Mathf.RoundToInt(end.direction);
        if (dir == 0 || dir == 180) return 2f;
        if (dir == 90 || dir == 270) return 0.5f;
        return -1f;
    }

    public void makeFakeDom(float v1, float v2, DominoScript end, int index, float val, UnityEngine.Vector3 offsetDir, DominoScript domino)
    {
        if (val != v1 && val != v2) { return; }

        UnityEngine.Vector3 pos = end.transform.position;

         float baseRotation = 0f;
      
        if (offsetDir.x > 0)
            baseRotation = 0f;

        else if (offsetDir.x < 0)
            baseRotation = 180f;

        else if (offsetDir.y > 0)
            baseRotation = 90f;

        else if (offsetDir.y < 0)
            baseRotation = 270f;
      
        bool isDouble = v1 == v2;

        if (!isDouble)
        {
            if (v2 == val)
            {
                baseRotation += 180f;
            }
        }
        else
        {
            baseRotation += 90f;
        }
        UnityEngine.Quaternion rotation = UnityEngine.Quaternion.Euler(0f, 0f, baseRotation);
         
        bool placingVertical = Mathf.Abs(offsetDir.y) > 0f;

        bool endIsVertical = Mathf.Abs(end.transform.eulerAngles.z % 180f) > 1f;

        float endHalf = placingVertical
        ? (endIsVertical ? 1f : 0.5f) 
        : (endIsVertical ? 0.5f : 1f); 

        float newHalf = placingVertical ? 1f : 1f; 
        
        float spacing = endHalf + newHalf;

        UnityEngine.Vector3 finalOffset = new UnityEngine.Vector3(
        Mathf.Sign(offsetDir.x) * (placingVertical ? 0f : spacing),
        Mathf.Sign(offsetDir.y) * (placingVertical ? spacing : 0f),
        0f
        );

        UnityEngine.Vector3 FakedomPosition = end.transform.position + finalOffset;
        GameObject fakeDomObj = Instantiate(FakeObject, FakedomPosition, rotation);
        fakeDomObj.GetComponent<FakeDom>().setup(v1, v2, domino.player, domino.gameObject, index);
    }

}