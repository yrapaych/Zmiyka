
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    SpriteRenderer sprite;
    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        transform.SetPositionAndRotation(new Vector3(Random.Range((int)-16, (int)16), Random.Range((int)-16, (int)16), 0), Quaternion.identity);
    }
    void NewPos()
    {
        transform.SetPositionAndRotation(new Vector3(Random.Range((int)-16, (int)16), Random.Range((int)-16, (int)16), 0), Quaternion.identity);
        sprite.enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (!sprite.enabled)
        {
            NewPos();
        }
    }
}
