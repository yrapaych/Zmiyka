
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    public GameObject head;
    // Start is called before the first frame update
   

    // Update is called once per frame
    void Update()
    {
        GetComponent<TextMesh>().text = head.GetComponent<HeadScript>().Bodys.Count.ToString();
        if (Input.GetKey(KeyCode.Escape))
        {
            Screen.fullScreen = false;
        }
    }
}
