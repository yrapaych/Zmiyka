using NEAT;
using UnityEngine;

class ButtonScript : MonoBehaviour
{
    private bool isRun = true;
    public bool enb = true;
    public GameObject head;
    public static NEAT.NEAT neat = new NEAT.NEAT(100);
    // Start is called before the first frame update
    void Awake()
    {
        StartGame();
    }

    public void StartGame()
    {
        GameObject.FindGameObjectWithTag("Head").GetComponent<HeadScript>().enabled = !this.isRun;
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Body"))
        {
            i.GetComponent<BodyScript>().enabled = !this.isRun;
        }
        this.isRun = !this.isRun;
        GameObject.FindGameObjectWithTag("Head").GetComponent<HeadScript>().genome = neat.champ();
    }

    
    // Update is called once per frame
    void Update()
    {

        
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider.name.Equals("ButtonStart")&&enb)
            {
                StartGame();
            }
            if (hit.collider.name.Equals("ButtonNewGame"))
            {
                ButtonNewGameScript.NewGame();
            }
            if (hit.collider.name.Equals("ButtonTrain10gen"))
            {
                GameObject.FindGameObjectWithTag("Train10Gen").transform.localScale *= 0.5f;
                neat.train(300);
                GameObject.FindGameObjectWithTag("Train10Gen").transform.localScale *= 2f;
            }
        }
    }
}
