using UnityEngine;
using System;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public enum Directions
{
    right, // Index 0
    left,  // Index 1
    up,    // Index 2
    down   // Index 3
}

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
    public GameObject[] Ends = new GameObject[4]; // 0: Right, 1: Left, 2: Up, 3: Down

    public TextMeshProUGUI yourteamText;
    public TextMeshProUGUI otherteamText;
    public TextMeshProUGUI curPlayerText;

    void Start()
    {
        Team1Score = 0;
        Team2Score = 0;
        PreRound();
        RoundStart();
        Display();
    }

    void Update()
    {
        if (Team1Score >= 610 || Team2Score >= 610)
        {
            EndGame();
        }
        if (playerWhosTurnItIsIndex == 0 || playerWhosTurnItIsIndex == 2)
        {
            yourteamText.text = "Your Team Has " + Team1Score + " points.";
            otherteamText.text = "Other Team Has " + Team2Score + " points.";
        }
        else
        {
            yourteamText.text = "Your Team Has " + Team2Score + " points.";
            otherteamText.text = "Other Team Has " + Team1Score + " points.";
        }
        curPlayerText.text = "Player " + (playerWhosTurnItIsIndex + 1) + "s turn";

    }

    // --- TURN LOGIC & VALIDATION ---

    public void nextPlayerTurn()
    {

        // 1. Calculate and add score ONCE per turn
        float moveScore = GetBoardEndTotal();
        Debug.Log("Score for this move: " + moveScore);

        Player currentPlayer = Players[playerWhosTurnItIsIndex].GetComponent<Player>();
        if (currentPlayer.dominos.Count == 0)
        {
            Debug.Log($"Player {playerWhosTurnItIsIndex} emptied their hand! Round Over.");
            AwardRemainingPoints(playerWhosTurnItIsIndex);
            return; // Stop the turn cycle
        }

        int playersChecked = 0;

        // Loop through players to find the next one who actually has a move
        while (playersChecked < Players.Length)
        {
            playerWhosTurnItIsIndex = (playerWhosTurnItIsIndex + 1) % Players.Length;

            if (HasAnyValidMoves(Players[playerWhosTurnItIsIndex]))
            {
                Display();
                return; // Valid player found, turn starts
            }

            Debug.Log($"Player {playerWhosTurnItIsIndex} has no moves. Skipping...");
            playersChecked++;
        }

        // If we exit the loop, it means NO player in the game has a valid move
        Debug.Log("Game Blocked: No players have legal moves left.");
        EndRoundBlocked();
    }

    public bool HasAnyValidMoves(GameObject player)
    {
        List<GameObject> hand = player.GetComponent<Player>().dominos;

        foreach (GameObject dominoObj in hand)
        {
            DominoScript script = dominoObj.GetComponent<DominoScript>();
            float v1 = script.side1Value;
            float v2 = script.side2Value;

            // Check against all 4 current board ends
            for (int i = 0; i < 4; i++)
            {
                if (Ends[i] == null) continue;

                // Cast index i to Directions enum to match your GetOpenValue logic
                float openValue = Ends[i].GetComponent<DominoScript>().GetOpenValue((Directions)i);

                if (v1 == openValue || v2 == openValue)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // --- SCORING & FAKES ---

    public float GetBoardEndTotal()
    {
        if (RootDomino == null) return 0;

        int total = 0;
        int rootOccurrences = 0;

        for (int i = 0; i < Ends.Length; i++)
        {
            if (Ends[i] == RootDomino) rootOccurrences++;
        }

        var root = RootDomino.GetComponent<DominoScript>();

        if (root.side1Value == root.side2Value) // Double
        {
            if (rootOccurrences >= 3) total += (root.side1Value + root.side2Value);
        }
        else
        {
            total += (root.side1Value + root.side2Value);
        }

        if (Ends[0] != null && Ends[0] != RootDomino) total += Ends[0].GetComponent<DominoScript>().GetOpenValue(Directions.right);
        if (Ends[1] != null && Ends[1] != RootDomino) total += Ends[1].GetComponent<DominoScript>().GetOpenValue(Directions.left);
        if (Ends[2] != null && Ends[2] != RootDomino) total += Ends[2].GetComponent<DominoScript>().GetOpenValue(Directions.up);
        if (Ends[3] != null && Ends[3] != RootDomino) total += Ends[3].GetComponent<DominoScript>().GetOpenValue(Directions.down);

        if (total % 5==0 && total > 0 )
        {
            if (playerWhosTurnItIsIndex == 0 || playerWhosTurnItIsIndex == 2)
            {
                Team1Score += total;

            }
            else
            {
                Team2Score += total;
            }
        }

        return total;



    }

    public void CheckValidMoves(DominoScript domino)
    {
        float v1 = domino.side1Value;
        float v2 = domino.side2Value;

        float rval = Ends[0].GetComponent<DominoScript>().GetOpenValue(Directions.right);
        makeFakeDom(v1, v2, Ends[0].GetComponent<DominoScript>(), 0, rval, new UnityEngine.Vector3(2f, 0f, 0f), domino);

        float lval = Ends[1].GetComponent<DominoScript>().GetOpenValue(Directions.left);
        makeFakeDom(v1, v2, Ends[1].GetComponent<DominoScript>(), 1, lval, new UnityEngine.Vector3(-2f, 0f, 0f), domino);

        float uval = Ends[2].GetComponent<DominoScript>().GetOpenValue(Directions.up);
        makeFakeDom(v1, v2, Ends[2].GetComponent<DominoScript>(), 2, uval, new UnityEngine.Vector3(0f, 2f, 0f), domino);

        float dval = Ends[3].GetComponent<DominoScript>().GetOpenValue(Directions.down);
        makeFakeDom(v1, v2, Ends[3].GetComponent<DominoScript>(), 3, dval, new UnityEngine.Vector3(0f, -2f, 0f), domino);
    }

    public void makeFakeDom(float v1, float v2, DominoScript end, int index, float val, UnityEngine.Vector3 offsetDir, DominoScript domino)
    {
        if (val != v1 && val != v2) return;

        float baseRotation = 0f;
        if (offsetDir.x > 0) baseRotation = 0f;
        else if (offsetDir.x < 0) baseRotation = 180f;
        else if (offsetDir.y > 0) baseRotation = 90f;
        else if (offsetDir.y < 0) baseRotation = 270f;

        bool isDouble = (v1 == v2);

        if (!isDouble)
        {
            if (v2 == val) baseRotation += 180f;
        }
        else
        {
            baseRotation += 90f;
        }

        UnityEngine.Quaternion rotation = UnityEngine.Quaternion.Euler(0f, 0f, baseRotation);

        bool placingVertical = Mathf.Abs(offsetDir.y) > 0f;
        bool endIsVertical = Mathf.Abs(end.transform.eulerAngles.z % 180f) > 1f;

        float endHalf = placingVertical ? (endIsVertical ? 1f : 0.5f) : (endIsVertical ? 0.5f : 1f);
        float spacing = endHalf + 1f;

        UnityEngine.Vector3 finalOffset = new UnityEngine.Vector3(
            Mathf.Sign(offsetDir.x) * (placingVertical ? 0f : spacing),
            Mathf.Sign(offsetDir.y) * (placingVertical ? spacing : 0f),
            0f
        );

        UnityEngine.Vector3 FakedomPosition = end.transform.position + finalOffset;

        GameObject fakeDomObj = Instantiate(FakeObject, FakedomPosition, rotation);
        fakeDomObj.GetComponent<FakeDom>().setup(v1, v2, domino.player, domino.gameObject, index);
    }

    // --- SETUP LOGIC ---

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

}

    public void RoundStart()
    {
        for (int i = 0; i < Players.Length; i++) CreateDominos(Players[i], 7);

        bool rootFound = false;
        for (int i = 0; i < Players.Length; i++)
        {
            var player = Players[i].GetComponent<Player>();
            for (int j = player.dominos.Count - 1; j >= 0; j--)
            {
                var domino = player.dominos[j];
                var cur = domino.GetComponent<DominoScript>();
                if (cur.side1Value == cur.side2Value)
                {
                    player.dominos.RemoveAt(j);
                    RootDomino = domino;

                    // Center the Root Domino at 0,0,0
                    RootDomino.transform.SetParent(null);
                    RootDomino.transform.position = UnityEngine.Vector3.zero;
                    RootDomino.transform.rotation = UnityEngine.Quaternion.identity;

                    Ends[0] = Ends[1] = Ends[2] = Ends[3] = RootDomino;
                    playerWhosTurnItIsIndex = i;
                    rootFound = true;

                    AntiZoom az = RootDomino.GetComponent<AntiZoom>();
                    if (az != null)
                    {
                        az.enabled = false;
                    }

                    Display();
                    nextPlayerTurn();
                    break;
                }
            }
            if (rootFound) break;
        }
    }

    public void CreateDominos(GameObject player, int howManyDominos)
    {
        for (int i = 0; i < howManyDominos; i++)
        {
            if (dominoVectorsList.Count == 0) break;
            UnityEngine.Vector3 pos = player.transform.position + offset * i;
            GameObject newDomino = Instantiate(DominoObject, pos, transform.rotation);
            newDomino.transform.SetParent(player.transform, true);
            player.GetComponent<Player>().dominos.Add(newDomino);

            int randomIndex = UnityEngine.Random.Range(0, dominoVectorsList.Count);
            System.Numerics.Vector2 data = dominoVectorsList[randomIndex];
            dominoVectorsList.RemoveAt(randomIndex);
            newDomino.GetComponent<DominoScript>().Setup((int)data.X, (int)data.Y, player);
        }
    }
    public void AwardRemainingPoints(int winnerIndex)
    {
        int bonusPoints = 0;

        // Loop through all players
        for (int i = 0; i < Players.Length; i++)
        {
            // Skip the winner (their hand is empty anyway)
            if (i == winnerIndex) continue;

            List<GameObject> hand = Players[i].GetComponent<Player>().dominos;
            foreach (GameObject domObj in hand)
            {
                DominoScript script = domObj.GetComponent<DominoScript>();
                // Sum up both sides of the domino
                bonusPoints += (int)(script.side1Value + script.side2Value);
            }
        }

        // Round to the nearest 5 (standard Domino rules usually round)
        // If you don't want to round, just remove the next line
        bonusPoints = Mathf.RoundToInt(bonusPoints / 5f) * 5;

        // Award to the correct team
        if (winnerIndex == 0 || winnerIndex == 2)
        {
            Team1Score += bonusPoints;
            Debug.Log("Team 1 wins round! Bonus: " + bonusPoints);
        }
        else
        {
            Team2Score += bonusPoints;
            Debug.Log("Team 2 wins round! Bonus: " + bonusPoints);
        }

        Debug.Log("Round Over. Starting next round...");
        ResetBoard();
        RoundStart();
    }
    void Display()
    {
        foreach (var player in Players) player.GetComponent<Player>().hideDominos();
        Players[playerWhosTurnItIsIndex].GetComponent<Player>().showDominos();
        if (RootDomino != null) RootDomino.SetActive(true);
    }

    void EndRoundBlocked()
    {
        int team1HandTotal = 0;
        int team2HandTotal = 0;

        // Calculate totals for both teams
        for (int i = 0; i < Players.Length; i++)
        {
            int playerTotal = 0;
            foreach (GameObject dom in Players[i].GetComponent<Player>().dominos)
            {
                var script = dom.GetComponent<DominoScript>();
                playerTotal += (int)(script.side1Value + script.side2Value);
            }

            if (i == 0 || i == 2) team1HandTotal += playerTotal;
            else team2HandTotal += playerTotal;
        }

        // Team with the LIGHTEST hand wins the points from the other team
        if (team1HandTotal < team2HandTotal)
        {
            Team1Score += (Mathf.RoundToInt(team2HandTotal / 5f) * 5);
        }
        else if (team2HandTotal < team1HandTotal)
        {
            Team2Score += (Mathf.RoundToInt(team1HandTotal / 5f) * 5);
        }

        // Reset for next round...
        ResetBoard();
        RoundStart();
    }
    
    public void ResetBoard()
    {
        // 1. Destroy EVERY object tagged as a Domino or Fake
        GameObject[] allDominos = GameObject.FindGameObjectsWithTag("Domino");
        foreach (GameObject d in allDominos) Destroy(d);

        GameObject[] allFakes = GameObject.FindGameObjectsWithTag("FakeDom");
        foreach (GameObject f in allFakes) Destroy(f);

        // 2. Clear the Player lists (the physical objects are already destroyed above)
        foreach (GameObject player in Players)
        {
            player.GetComponent<Player>().dominos.Clear();
        }

        // 3. Null out our tracking variables
        RootDomino = null;
        for (int i = 0; i < Ends.Length; i++)
        {
            Ends[i] = null;
        }

        // 4. Refresh the data list
        PreRound();
    }

    void EndGame() { /* Handle scoring/reset here */ }
    void PrintEndsIfChanged() { }
}