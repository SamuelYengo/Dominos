using UnityEngine;
using System;
using System.Numerics;
using System.Collections.Generic;
public enum Directions
{
    right,
    left,
    up,
    down,
}

public class GameManager : MonoBehaviour
{
    public System.Numerics.Vector2 Domino;
    public List<System.Numerics.Vector2> dominoVectorsList;

    public GameObject DominoObject;
    public GameObject FakeObject;
    public GameObject[] Players;

    public int Team1Score;
    public int Team2Score;

    public bool roundStart;
    public UnityEngine.Vector3 offset;
    public int playerWhosTurnItIsIndex;
    public GameObject RootDomino;
    public GameObject[] Ends;

    void Start()
    {
        PreRound();
        RoundStart();
        Display();
    }
    string lastEndsState = "";

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
    /* public float GetBoardEndTotal(Directions dir)
     {
         float total = 0;
         HashSet<GameObject> counted = new HashSet<GameObject>();

         for (int i = 0; i < Ends.Length; i++)
         {
             GameObject endObj = Ends[i];

             if (endObj == null)
                 continue;

             if (counted.Contains(endObj))
                 continue;

             counted.Add(endObj);

             var domino = endObj.GetComponent<DominoScript>();

             int occurrences = 0;
             for (int j = 0; j < Ends.Length; j++)
             {
                 if (Ends[j] == endObj)
                     occurrences++;
             }

             bool isDouble = domino.side1Value == domino.side2Value;

             // If a double has exactly two connections, it's closed and shouldn't count
             if (isDouble && occurrences == 2)
                 continue;

             if (endObj == RootDomino)
             {
                 total += domino.side1Value + domino.side2Value;
             }
             else
             {
                 total += GetEndValue(endObj, dir);
             }
         }
         return total;
     }
    */
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
                return end.UpValue;

            case 180:
                return end.DownValue;

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
                return end.DownValue;

            case 180:
                return end.UpValue;

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
                return end.UpValue;
            case 90:
                return end.DownValue;

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
                return end.UpValue;

            case 270:
                return end.DownValue;

            default:
                Debug.LogError("Invalid direction");
                return -1;
        }
    }

    void PrintEndsIfChanged()
    {
        string currentState =
            $"up: {Ends[2].GetComponent<DominoScript>().UpValue} | {Ends[2].GetComponent<DominoScript>().DownValue}, " +
            $"down: {Ends[3].GetComponent<DominoScript>().UpValue} | {Ends[3].GetComponent<DominoScript>().DownValue}, " +
            $"left: {Ends[1].GetComponent<DominoScript>().UpValue} | {Ends[1].GetComponent<DominoScript>().DownValue}, " +
            $"right: {Ends[0].GetComponent<DominoScript>().UpValue} | {Ends[0].GetComponent<DominoScript>().DownValue}";

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
            // Safety check in case we run out of dominos
            if (dominoVectorsList.Count == 0) break;

            UnityEngine.Vector3 dominoPosition = player.transform.position + offset * i;

            GameObject newDomino = Instantiate(DominoObject, dominoPosition, transform.rotation);
            newDomino.transform.SetParent(player.transform, true);
            player.GetComponent<Player>().dominos.Add(newDomino);

            var dominoScript = newDomino.GetComponent<DominoScript>();

            // --- MOVED LOGIC HERE ---
            // 1. Pick random index (Note: Random.Range int is exclusive max, so use Count, not Count-1)
            int randomIndex = UnityEngine.Random.Range(0, dominoVectorsList.Count);

            // 2. Get data
            System.Numerics.Vector2 randomDomino = dominoVectorsList[randomIndex];

            // 3. Remove from list
            dominoVectorsList.RemoveAt(randomIndex);

            // 4. Initialize the script immediately so values are ready for checking
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

        // Find the heaviest double
        // Note: The original logic looked for *any* double. Usually, domino rules look for the highest double (6|6).
        // This preserves your original logic of finding the first one encountered in the loops.
        for (int i = 0; i < Players.Length; i++)
        {
            var player = Players[i].GetComponent<Player>();

            // Iterate backwards so we can remove safely
            for (int j = player.dominos.Count - 1; j >= 0; j--)
            {
                var domino = player.dominos[j];
                var cur = domino.GetComponent<DominoScript>();

                // Compare numeric values now (string comparison works too but floats are safer)
                float v1 = cur.side1Value;
                float v2 = cur.side2Value;


                if (v1 == v2) // It's a double
                {
                    // Logic to see if this is the double we want (currently accepts the first one found)
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

        //var Domino = DominoObject.GetComponent<DominoScript>();
        //var v1 = Domino.side1Value;
        //var v2 = Domino.side2Value;
        float v1 = domino.side1Value;
        float v2 = domino.side2Value;
        var rightEnd = Ends[0].GetComponent<DominoScript>();
        float rval = -1;
        int Rdir = Mathf.RoundToInt(rightEnd.direction);
        switch (Rdir)
        {
            case 0:
            case 90:
            case 270:
                rval = rightEnd.side2Value;
                break;
            case 180:
                rval = rightEnd.side1Value;
                break;
            default:
                Debug.LogError("ahhhh");
                break;
        }
        //makeFakeDom(v1, v2, rightEnd, rval, false, new UnityEngine.Vector3(2f, 0f,0f));
        makeFakeDom(v1, v2, rightEnd, 0, rval, new UnityEngine.Vector3(2f, 0f, 0f), domino);

        var LeftEnd = Ends[1].GetComponent<DominoScript>();

        float lval = -1;
        int Ldir = Mathf.RoundToInt(LeftEnd.direction);
        switch (Ldir)
        {
            case 0:
            case 90:
            case 270:
                lval = LeftEnd.side1Value;
                break;
            case 180:
                lval = LeftEnd.side2Value;
                break;
            default:
                Debug.LogError("ahhhh");
                break;
        }
        //makeFakeDom(v1, v2, LeftEnd, lval, false, new UnityEngine.Vector3(-2f, 0f, 0f));
        makeFakeDom(v1, v2, LeftEnd, 1, lval, new UnityEngine.Vector3(-2f, 0f, 0f), domino);

        var UpEnd = Ends[2].GetComponent<DominoScript>();
        float Uval = -1;
        int Udir = Mathf.RoundToInt(UpEnd.direction);
        switch (Udir)
        {
            case 0:
            case 180:
            case 270:
                Uval = UpEnd.side2Value;
                break;
            case 90:
                Uval = UpEnd.side1Value;
                break;
            default:
                Debug.LogError("ahhhh");
                break;
        }
        //makeFakeDom(v1, v2, UpEnd, Uval, true, new UnityEngine.Vector3(0f, 2f, 0f));
        makeFakeDom(v1, v2, UpEnd, 2, Uval, new UnityEngine.Vector3(0f, 2f, 0f), domino);

        var DownEnd = Ends[3].GetComponent<DominoScript>();
        float Dval = -1;
        int Ddir = Mathf.RoundToInt(DownEnd.direction);
        switch (Ddir)
        {
            case 0:
            case 180:
            case 90:
                Dval = DownEnd.side2Value;
                break;
            case 270:
                Dval = DownEnd.side1Value;
                break;
            default:
                Debug.LogError("ahhhh");
                break;
        }
        //makeFakeDom(v1, v2, DownEnd, Dval, true, new UnityEngine.Vector3(0f, -2f, 0f));
        makeFakeDom(v1, v2, DownEnd, 3, Dval, new UnityEngine.Vector3(0f, -2f, 0f), domino);



    }

    public float getOffset(DominoScript end)
    {
        int dir = Mathf.RoundToInt(end.direction);
        switch (dir)
        {
            case 0:
            case 180:
                return 2f;
            case 90:
            case 270:
                return 0.5f;
            default:
                return -1f;
        }
    }

    public void makeFakeDom(float v1, float v2, DominoScript end, int index, float val, UnityEngine.Vector3 offsetDir, DominoScript domino)
    {
        if (val != v1 && val != v2)
            return;

        UnityEngine.Vector3 pos = end.transform.position;
         //UnityEngine.Vector3 FakedomPosition = pos + offsetDir;

         float baseRotation = 0f;
        /*
         if (offsetDir == new UnityEngine.Vector3(2f, 0f, 0f))        
             baseRotation = 0f;

         else if (offsetDir == new UnityEngine.Vector3(-2f, 0f, 0f))  
             baseRotation = 180f;

         else if (offsetDir == new UnityEngine.Vector3(0f, 2f, 0f))   
             baseRotation = 90f;

         else if (offsetDir == new UnityEngine.Vector3(0f, -2f, 0f))  
             baseRotation = 270f;
        */
        if (offsetDir.x > 0)
            baseRotation = 0f;

        else if (offsetDir.x < 0)
            baseRotation = 180f;

        else if (offsetDir.y > 0)
            baseRotation = 90f;

        else if (offsetDir.y < 0)
            baseRotation = 270f;
        /*
       if (val == v2)
           baseRotation += 180f;

       if (v1 == v2)
           baseRotation += 90f;
      

        // Determine which side connects to the board
        bool side1Matches = (v1 == val);
        bool side2Matches = (v2 == val);

        // rotate so the matching side touches the board
        if (side1Matches && !side2Matches)
        {
            baseRotation += 180f;
        }
        else if (side1Matches && side2Matches)
        {
            // double
            baseRotation += 90f;
        }
        */
        bool isDouble = (v1 == v2);

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
         
        // Determine placement orientation
        bool placingVertical = Mathf.Abs(offsetDir.y) > 0f;

        // Determine if the end domino is vertical
        bool endIsVertical = Mathf.Abs(end.transform.eulerAngles.z % 180f) > 1f;

        // Compute half-lengths along placement direction
        float endHalf = placingVertical
        ? (endIsVertical ? 1f : 0.5f) // vertical placement uses end's height/2
        : (endIsVertical ? 0.5f : 1f); // horizontal placement uses end's width/2

        float newHalf = placingVertical ? 1f : 1f; // new domino length in placement direction is always 2 units total, so half = 1
        
        // Compute spacing
        float spacing = endHalf + newHalf;

        // Final offset from end domino center
        UnityEngine.Vector3 finalOffset = new UnityEngine.Vector3(
        Mathf.Sign(offsetDir.x) * (placingVertical ? 0f : spacing),
        Mathf.Sign(offsetDir.y) * (placingVertical ? spacing : 0f),
        0f
        );

        // Position for the new domino
        UnityEngine.Vector3 FakedomPosition = end.transform.position + finalOffset;


        GameObject fakeDomObj = Instantiate(FakeObject, FakedomPosition, rotation);
        fakeDomObj.GetComponent<FakeDom>().setup(v1, v2, domino.player, domino.gameObject, index);
    }

}