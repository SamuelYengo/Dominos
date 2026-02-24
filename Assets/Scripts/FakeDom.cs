using UnityEngine;
using TMPro;

public class FakeDom : MonoBehaviour
{
    public GameObject side1;
    public GameObject side2;
    public TextMeshPro text1;
    public TextMeshPro text2;
    public GameManager gameManager;

    public float FDside1Value;
    public float FDside2Value;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void setup(float v1, float v2)
    {
        FDside1Value = v1;
        FDside2Value = v2;
        text1.text = "" + FDside1Value;
        text2.text = "" + FDside2Value;
        
        text1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        text2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButtonClick()
    {
        // make real, remove from players hand
    }
}
