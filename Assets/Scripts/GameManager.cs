using UnityEngine;
using System;
using System.Numerics;
using System.Collections.Generic;

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
        makeFakeDom(v1, v2, rightEnd, rval, new UnityEngine.Vector3(2f, 0f, 0f), domino);

        var LeftEnd = Ends[1].GetComponent<DominoScript>();

        float lval = -1;
        int Ldir = Mathf.RoundToInt(LeftEnd.direction);
        switch (Ldir)
        {
            case 0:
            case 90:
            case 270:
                lval = LeftEnd.side2Value;
                break;
            case 180:
                lval = LeftEnd.side1Value;
                break;
            default:
                Debug.LogError("ahhhh");
                break;
        }
        //makeFakeDom(v1, v2, LeftEnd, lval, false, new UnityEngine.Vector3(-2f, 0f, 0f));
        makeFakeDom(v1, v2, LeftEnd, lval, new UnityEngine.Vector3(-2f, 0f, 0f), domino);

        var UpEnd = Ends[2].GetComponent<DominoScript>();
        float Uval = -1;
        int Udir = Mathf.RoundToInt(UpEnd.direction);
        switch (Udir)
        {
            case 0:
            case 90:
            case 270:
                Uval = UpEnd.side2Value;
                break;
            case 180:
                Uval = UpEnd.side1Value;
                break;
            default:
                Debug.LogError("ahhhh");
                break;
        }
        //makeFakeDom(v1, v2, UpEnd, Uval, true, new UnityEngine.Vector3(0f, 2f, 0f));
        makeFakeDom(v1, v2, UpEnd, Uval, new UnityEngine.Vector3(0f, 2f, 0f), domino);

        var DownEnd = Ends[3].GetComponent<DominoScript>();
        float Dval = -1;
        int Ddir = Mathf.RoundToInt(DownEnd.direction);
        switch (Ddir)
        {
            case 0:
            case 90:
            case 270:
                Dval = DownEnd.side2Value;
                break;
            case 180:
                Dval = DownEnd.side1Value;
                break;
            default:
                Debug.LogError("ahhhh");
                break;
        }
        //makeFakeDom(v1, v2, DownEnd, Dval, true, new UnityEngine.Vector3(0f, -2f, 0f));
        makeFakeDom(v1, v2, DownEnd, Dval, new UnityEngine.Vector3(0f, -2f, 0f), domino);



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

    public void makeFakeDom(float v1, float v2, DominoScript end, float val, UnityEngine.Vector3 offsetDir, DominoScript domino)
    {
        if (val != v1 && val != v2)
            return;

        UnityEngine.Vector3 pos = end.transform.position;
         //UnityEngine.Vector3 FakedomPosition = pos + offsetDir;

         float baseRotation = 0f;

         if (offsetDir == new UnityEngine.Vector3(2f, 0f, 0f))        
             baseRotation = 0f;

         else if (offsetDir == new UnityEngine.Vector3(-2f, 0f, 0f))  
             baseRotation = 180f;

         else if (offsetDir == new UnityEngine.Vector3(0f, 2f, 0f))   
             baseRotation = 90f;

         else if (offsetDir == new UnityEngine.Vector3(0f, -2f, 0f))  
             baseRotation = 270f;

         if (val == v2)
             baseRotation += 180f;

         if (v1 == v2)
             baseRotation += 90f;
        
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
        fakeDomObj.GetComponent<FakeDom>().setup(v1, v2, domino.player, domino.gameObject);
    }

}