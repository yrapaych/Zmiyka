using System.IO;
using System;
using UnityEngine;

public class BestScoreScript : MonoBehaviour
{
    public int BestScore = 0;

    void Awake()
    {/*
        StreamReader stream = new StreamReader("Zmiyka_Data/BestScore.txt");
        BestScore = Convert.ToInt32(stream.ReadLine());
        GetComponent<TextMesh>().text = BestScore.ToString();
        stream.Close();*/
    }
    public void SetScore(int Score)
    {/*
        if(BestScore < Score)
        {
            BestScore = Score;
            GetComponent<TextMesh>().text = BestScore.ToString();
            StreamWriter writer = new StreamWriter("Zmiyka_Data/BestScore.txt", false);
            writer.Write(BestScore.ToString());
            writer.Close();
        }*/
    }
}
