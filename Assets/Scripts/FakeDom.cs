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
    public GameObject end;

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
    public void setup(float v1, float v2, GameObject player, GameObject ogDom)
    {
        FDside1Value = v1;
        FDside2Value = v2;
        text1.text = "" + FDside1Value;
        text2.text = "" + FDside2Value;
        
        text1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        text2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        this.player = player;
        this.ogDomino = ogDom;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseDown()
    {
        ogDomino.transform.position = this.transform.position;
        ogDomino.transform.rotation = this.transform.rotation;
        ogDomino.GetComponent<DominoScript>().text1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        ogDomino.GetComponent<DominoScript>().text2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        player.GetComponent<Player>().dominos.Remove(ogDomino);
        ogDomino.GetComponent<DominoScript>().ClearFakeDoms();
        gameManager.nextPlayerTurn();



    }
}
