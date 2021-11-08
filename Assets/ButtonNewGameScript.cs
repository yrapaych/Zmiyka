
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonNewGameScript : MonoBehaviour
{


    public static void NewGame()
    {
        GameObject.FindGameObjectWithTag("BestScore").GetComponent<BestScoreScript>().SetScore(GameObject.FindGameObjectWithTag("Head").GetComponent<HeadScript>().Bodys.Count);
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
    // Update is called once per frame
    void Update()
    {
       
    }
}
