
using System.Collections.Generic;
using UnityEngine;
using NEAT;

class HeadScript : MonoBehaviour
{
    public Genome genome = ButtonScript.neat.champ();
    double[] Inputs = new double[14];
    GameObject food;


    public GameObject prev;
    public List<GameObject> Bodys;
    private Vector2 dir = new Vector2(0, 1);
    private Vector2 lastDir = new Vector2(0, 1);
    private double time = 0;
    private double maxTime = 0.2;
    private bool grow;
    // Start is called before the first frame update
    public void upChamp(Genome champ)
    {
        genome = champ;
    }
    void Awake()
    {
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Body"))
        {
            Bodys.Add(i);
        }
        food = GameObject.FindGameObjectWithTag("Food");
    }
    double lengthXY(double x1, double y1, double x2, double y2)
    {
        return (System.Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
    }
    void GetInput()
    {
        Inputs[0] = Physics2D.Raycast(transform.position, new Vector2(0, -1)).distance;
        Inputs[1] = Physics2D.Raycast(transform.position, new Vector2(0, 1)).distance;
        Inputs[2] = Physics2D.Raycast(transform.position, new Vector2(-1, 0)).distance;
        Inputs[3] = Physics2D.Raycast(transform.position, new Vector2(1, 0)).distance;
        Inputs[4] = Physics2D.Raycast(transform.position, new Vector2(1, 1)).distance;
        Inputs[5] = Physics2D.Raycast(transform.position, new Vector2(-1, -1)).distance;
        Inputs[6] = Physics2D.Raycast(transform.position, new Vector2(-1, 1)).distance;
        Inputs[7] = Physics2D.Raycast(transform.position, new Vector2(1, -1)).distance;

        float x1 = ((Vector2)transform.position).x;
        float y1 = ((Vector2)transform.position).y;
        if (x1 == ((Vector2)food.transform.position).x)
        {
            Inputs[8] = ((Vector2)food.transform.position).y - y1;
        }
        else Inputs[8] = 0;
        if (y1 == ((Vector2)food.transform.position).y)
        {
            Inputs[9] = ((Vector2)food.transform.position).x - x1;
        }
        else
        {
            Inputs[9] = 0;
        }
        x1 -= 16;
        y1 -= 16;
        while (x1 < 16 && y1 < 16)
        {
            if (((Vector2)food.transform.position).x == x1 && ((Vector2)food.transform.position).y == y1)
            {
                if (((Vector2)food.transform.position).y > ((Vector2)transform.position).y)
                {
                    Inputs[10] = lengthXY(((Vector2)transform.position).x, ((Vector2)transform.position).y, ((Vector2)food.transform.position).x, ((Vector2)food.transform.position).y);
                }
                else
                {
                    Inputs[10] = -lengthXY(((Vector2)transform.position).x, ((Vector2)transform.position).y, ((Vector2)food.transform.position).x, ((Vector2)food.transform.position).y);
                }
                break;
            }
            else
            {
                Inputs[10] = 0;
            }
            if (((Vector2)food.transform.position).x == -x1 && ((Vector2)food.transform.position).y == y1)
            {
                if (((Vector2)food.transform.position).y > ((Vector2)transform.position).y)
                {
                    Inputs[11] = lengthXY(((Vector2)transform.position).x, ((Vector2)transform.position).y, ((Vector2)food.transform.position).x, ((Vector2)food.transform.position).y);
                }
                else
                {
                    Inputs[11] = -lengthXY(((Vector2)transform.position).x, ((Vector2)transform.position).y, ((Vector2)food.transform.position).x, ((Vector2)food.transform.position).y);
                }
                break;
            }
            else
            {
                Inputs[11] = 0;
            }
            x1++;
            y1++;
        }
        Inputs[12] = (((Vector2)food.transform.position).x - ((Vector2)transform.position).x);
        Inputs[13] = (((Vector2)food.transform.position).y - ((Vector2)transform.position).y);
        genome.Input(Inputs);
        double[] put = genome.output();
        int Move = 0;
        double max = 0;
        for(int i = 0; i < put.Length; i++)
        {
            if (max < put[i])
            {
                max = put[i];
                Move = i;
            }
        }
        for(int i = 0; i < 14; i++)
        {
            print(Inputs[i]);
        }
        Vector2 buf = new Vector2(dir.x, dir.y);
        if (Move == 0)
        {
            buf.Set(-1, 0);
        }
        else if (Move == 1)
        {
            buf.Set(1, 0);
        }
        else if (Move == 2)
        {
            buf.Set(0, 1);
        }
        else if (Move == 3)
        {
            buf.Set(0, -1);
        }
        if (buf.x != lastDir.x && buf.y != lastDir.y)
        {
            dir.Set(buf.x, buf.y);
        }

    }

    void Lose()
    {
        GameObject.FindGameObjectWithTag("Lose").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.FindGameObjectWithTag("ButtonStart").GetComponent<ButtonScript>().enb = false;
        GameObject.FindGameObjectWithTag("ButtonStart").GetComponent<ButtonScript>().StartGame();
        GameObject.FindGameObjectWithTag("BestScore").GetComponent<BestScoreScript>().SetScore(Bodys.Count);
    }

    void EatFood()
    {
        if (food.transform.position == transform.position)
        {
            food.GetComponent<SpriteRenderer>().enabled = false;
            grow = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        GetInput();
        EatFood();
        time += Time.deltaTime;
        if (time > maxTime)
        {
            if (grow)
            {
                GameObject obj = Instantiate(Resources.Load("Body"), transform.position, Quaternion.identity) as GameObject;
                BodyScript newB = obj.GetComponent<BodyScript>();
                newB.next = gameObject;
                newB.prev = this.prev;
                this.prev.GetComponent<BodyScript>().next = newB.gameObject;
                this.prev = newB.gameObject;
                Bodys.Add(newB.gameObject);
                transform.Translate(dir);
                grow = false;
                time = 0;
            }
            else
            {


                foreach (GameObject i in Bodys)
                {
                    i.transform.Translate(i.GetComponent<BodyScript>().next.transform.position - i.transform.position);
                }
                transform.Translate(dir);
                time = 0;

            }
            lastDir.Set(dir.x, dir.y);
        }
        foreach(GameObject i in GameObject.FindGameObjectsWithTag("Body"))
        {
            if (i.transform.position == transform.position) Lose();
        }
        if(transform.position.x > 16 || transform.position.x < -16 || transform.position.y > 16 || transform.position.y < -16)
        {
            Lose();
        }
    }
}
