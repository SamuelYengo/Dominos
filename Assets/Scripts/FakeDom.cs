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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        float rot = ogDomino.transform.eulerAngles.z;

        // normalize rotation so it's always 0,90,180,270
        rot = Mathf.Round(rot / 90f) * 90f;

        dom.direction = rot;

        // Get the value we are connecting to BEFORE changing anything
        float connectedValue = -1;

        switch (endIndex)
        {
            case 0: connectedValue = gameManager.GetEndValue(gameManager.Ends[0], Directions.right); break;
            case 1: connectedValue = gameManager.GetEndValue(gameManager.Ends[1], Directions.left); break;
            case 2: connectedValue = gameManager.GetEndValue(gameManager.Ends[2], Directions.up); break;
            case 3: connectedValue = gameManager.GetEndValue(gameManager.Ends[3], Directions.down); break;
        }

        dom.text1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        dom.text2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        player.GetComponent<Player>().dominos.Remove(ogDomino);

        // clear fake dominos AFTER we got the board data
        dom.ClearFakeDoms();

        // update the board end
        float v1 = dom.side1Value;
        float v2 = dom.side2Value;

        float newOpenValue;

        if (v1 == connectedValue)
            newOpenValue = v2;
        else
            newOpenValue = v1;

        // store values so board logic reads correctly
        dom.UpValue = newOpenValue;
        dom.DownValue = connectedValue;
        // update the board end
        gameManager.Ends[endIndex] = ogDomino;

        // store correct open side values

        ogDomino.SetActive(true);

        float total = gameManager.GetBoardEndTotal();
        Debug.Log("Board total = " + total);

        gameManager.nextPlayerTurn();
    }
}
