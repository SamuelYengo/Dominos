using UnityEngine;
using TMPro;

public class FakeDom : MonoBehaviour
{
    public GameObject player;
    public GameObject ogDomino;
    public GameObject side1;
    public GameObject side2;
    public TextMeshPro text1;
    public TextMeshPro text2;
    public GameManager gameManager;
    public int endIndex;

    public float FDside1Value;
    public float FDside2Value;
    void Start()
    {

    }
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }
    public void setup(float v1, float v2, GameObject player, GameObject ogDom, int endIndex)
    {
        FDside1Value = v1;
        FDside2Value = v2;
        text1.text = "" + FDside1Value;
        text2.text = "" + FDside2Value;
        
        text1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        text2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        this.player = player;
        this.ogDomino = ogDom;
        this.endIndex = endIndex;
    }

    // Update is called once per frame
    void Update()
    {

    }
       public void OnMouseDown()
    {
        ogDomino.transform.SetParent(null, true);
        ogDomino.transform.position = this.transform.position;
        ogDomino.transform.rotation = this.transform.rotation;


        var dom = ogDomino.GetComponent<DominoScript>();
        dom.text1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        dom.text2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        // 1. Remove from hand
        player.GetComponent<Player>().dominos.Remove(ogDomino);
        dom.ClearFakeDoms();

        // 2. Update the Board Ends array
        // This is crucial: the domino we just placed IS now the new end for that direction
        gameManager.Ends[endIndex] = ogDomino;

        // 3. Force an update of the internal values (UpValue/DownValue)
        // Your DominoScript.Update() handles the Up/Down values based on position,
        // but we should ensure the object is active so Update runs.
        ogDomino.SetActive(true);

        // 4. Calculate Score
        float total = gameManager.GetBoardEndTotal();
        Debug.Log("Board total = " + total);

        gameManager.nextPlayerTurn();
    }
}

