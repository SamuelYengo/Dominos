using UnityEngine;
using System.Collections.Generic;

public class tutorialScript : MonoBehaviour
{
    public List<GameObject> screenshots;
    private int currentIndex = 0; // Keeps track of the active screenshot

    void Start()
    {
        // Safety check: make sure the list isn't empty
        if (screenshots.Count > 0)
        {
            // Set only the first one to active, hide the rest
            for (int i = 0; i < screenshots.Count; i++)
            {
                screenshots[i].SetActive(i == 0);
            }
        }
    }

    public void nextPart()
    {
        if (screenshots.Count < 2) return; // Need at least 2 items to "switch"

        // 1. Hide the current one
        screenshots[currentIndex].SetActive(false);

        // 2. Move to the next index (loops back to 0 if at the end)
        currentIndex = (currentIndex + 1) % screenshots.Count;

        // 3. Show the new current one
        screenshots[currentIndex].SetActive(true);
    }
}
