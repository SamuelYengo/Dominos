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

    public Color scoringEndColor = Color.yellow; // Customize this in the Inspector
    private List<DominoScript> currentlyHighlighted = new List<DominoScript>();
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
        // 1. CLEAR PREVIOUS HIGHLIGHTS
        // We do this first so that dominoes that are no longer "ends" return to normal
        foreach (var dom in currentlyHighlighted)
        {
            if (dom != null)
            {
                // Pass false and any direction to reset colors to white
                dom.HighlightScoringSide(false, Color.white, Directions.right);
            }
        }
        currentlyHighlighted.Clear();

        if (RootDomino == null) return 0;

        int total = 0;
        int rootOccurrences = 0;

        // Count how many directions are currently attached to the root
        for (int i = 0; i < Ends.Length; i++)
        {
            if (Ends[i] == RootDomino) rootOccurrences++;
        }

        DominoScript rootScript = RootDomino.GetComponent<DominoScript>();

        // 2. CALCULATE & HIGHLIGHT ROOT SCORE
        bool rootIsScoring = false;
        if (rootScript.side1Value == rootScript.side2Value) // Double
        {
            // In many rules, the root double only scores if it's still an open end (3+ sides open)
            if (rootOccurrences >= 3)
            {
                total += (int)(rootScript.side1Value + rootScript.side2Value);
                rootIsScoring = true;
            }
        }
        else
        {
            // If the root isn't a double, it scores both sides initially
            total += (int)(rootScript.side1Value + rootScript.side2Value);
            rootIsScoring = true;
        }

        if (rootIsScoring)
        {
            // Root highlights both sides because both sides contribute to the initial score
            rootScript.HighlightScoringSide(true, scoringEndColor, Directions.right);
            currentlyHighlighted.Add(rootScript);
        }

        // 3. CALCULATE & HIGHLIGHT OTHER ENDS
        for (int i = 0; i < 4; i++)
        {
            // Only score if the end is not empty and not the Root (handled above)
            if (Ends[i] != null && Ends[i] != RootDomino)
            {
                DominoScript script = Ends[i].GetComponent<DominoScript>();
                Directions dir = (Directions)i;

                // Highlight the side facing OUTward
                script.HighlightScoringSide(true, scoringEndColor, dir);
                currentlyHighlighted.Add(script);

                // Calculation Logic
                if (script.side1Value == script.side2Value)
                {
                    // Doubles count as the sum of both sides (e.g., 5|5 = 10)
                    total += (int)(script.side1Value + script.side2Value);
                }
                else
                {
                    // Normal dominoes only count the "open" value facing the board edge
                    total += script.GetOpenValue(dir);
                }
            }
        }

        // 4. AWARD POINTS
        // Only multiples of 5 score points in standard All-Fives rules
        if (total % 5 == 0 && total > 0)
        {
            if (playerWhosTurnItIsIndex == 0 || playerWhosTurnItIsIndex == 2)
            {
                Team1Score += total;
                Debug.Log($"Team 1 scored {total} points!");
            }
            else
            {
                Team2Score += total;
                Debug.Log($"Team 2 scored {total} points!");
            }
        }

        return total;
    }

    public void CheckValidMoves(DominoScript domino)
    {
        float v1 = domino.side1Value;
        float v2 = domino.side2Value;

        // Use 1f as the multiplier since makeFakeDom now calculates the exact spacing
        float rval = Ends[0].GetComponent<DominoScript>().GetOpenValue(Directions.right);
        makeFakeDom(v1, v2, Ends[0].GetComponent<DominoScript>(), 0, rval, new UnityEngine.Vector3(1f, 0f, 0f), domino);

        float lval = Ends[1].GetComponent<DominoScript>().GetOpenValue(Directions.left);
        makeFakeDom(v1, v2, Ends[1].GetComponent<DominoScript>(), 1, lval, new UnityEngine.Vector3(-1f, 0f, 0f), domino);

        float uval = Ends[2].GetComponent<DominoScript>().GetOpenValue(Directions.up);
        makeFakeDom(v1, v2, Ends[2].GetComponent<DominoScript>(), 2, uval, new UnityEngine.Vector3(0f, 1f, 0f), domino);

        float dval = Ends[3].GetComponent<DominoScript>().GetOpenValue(Directions.down);
        makeFakeDom(v1, v2, Ends[3].GetComponent<DominoScript>(), 3, dval, new UnityEngine.Vector3(0f, -1f, 0f), domino);
    }

    public void makeFakeDom(float v1, float v2, DominoScript end, int index, float val, UnityEngine.Vector3 offsetDir, DominoScript domino)
    {
        if (val != v1 && val != v2) return;

        // 1. Calculate Rotation based on direction
        float baseRotation = 0f;
        if (offsetDir.x > 0) baseRotation = 0f;      // Right
        else if (offsetDir.x < 0) baseRotation = 180f; // Left
        else if (offsetDir.y > 0) baseRotation = 90f;   // Up
        else if (offsetDir.y < 0) baseRotation = 270f; // Down

        bool isDouble = (v1 == v2);
        bool endIsDouble = (end.side1Value == end.side2Value);

        if (!isDouble)
        {
            if (v2 == val) baseRotation += 180f;
        }
        else
        {
            // Doubles are placed cross-wise (rotated 90 degrees from the line)
            baseRotation += 90f;
        }

        UnityEngine.Quaternion rotation = UnityEngine.Quaternion.Euler(0f, 0f, baseRotation);

        // 2. Precision Spacing Logic
        float endExtent = 0f;
        float newExtent = 0f;
        bool isHorizontalPath = (index == 0 || index == 1);

        // --- Calculate End Domino "Thickness" facing the connection ---
        if (end.gameObject == RootDomino)
        {
            // Root is horizontal. 
            // Distance to Left/Right edge is 1.0. Distance to Top/Bottom edge is 0.5.
            endExtent = isHorizontalPath ? 1.0f : 0.5f;
        }
        else if (endIsDouble)
        {
            // All other doubles are placed cross-wise, so thickness is always 0.5
            endExtent = 0.5f;
        }
        else
        {
            // Normal dominoes follow the line, so thickness is always 1.0
            endExtent = 1.0f;
        }

        // --- Calculate New Domino "Thickness" facing the connection ---
        if (isDouble)
        {
            // New double will be cross-wise (0.5)
            newExtent = 0.5f;
        }
        else
        {
            // New normal will follow the line (1.0)
            newExtent = 1.0f;
        }

        float spacing = endExtent + newExtent;

        // 3. Final Position Calculation
        // Multiply the unit direction by our calculated spacing
        UnityEngine.Vector3 finalOffset = new UnityEngine.Vector3(
            (offsetDir.x != 0) ? offsetDir.x * spacing : 0f,
            (offsetDir.y != 0) ? offsetDir.y * spacing : 0f,
            0f
        );

        UnityEngine.Vector3 FakedomPosition = end.transform.position + finalOffset;

        // 4. Instantiate
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
            //UnityEngine.Vector3 pos = player.transform.position + offset * i;
            GameObject newDomino = Instantiate(DominoObject, player.transform.position, transform.rotation);
            newDomino.transform.SetParent(player.transform);

            // 2. Setup the "Desired" position (as if zoom was 1.0)
            UnityEngine.Vector3 desiredLocalPos = offset * i;

            // 3. Tell AntiZoom to take it from here
            AntiZoom az = newDomino.GetComponent<AntiZoom>();
            if (az != null)
            {
                // This is the magic: we pass the desired position to the script
                // and let the script handle the zoom multiplication.
                az.SetBasePosition(desiredLocalPos);
            }
            else
            {
                // Fallback if no script
                newDomino.transform.localPosition = desiredLocalPos;
            }
            //GameObject newDomino = Instantiate(DominoObject, pos, transform.rotation);
            //newDomino.transform.SetParent(player.transform, true);
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