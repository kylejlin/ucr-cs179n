using UnityEngine;

public class MobileCreature : Creature
{
    public bool canSwim = false;
    public int consumeRate;
    public float speed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print(FindClosest<MobileCreature>(new Vector3(0,0,0).transform.localPosition.x);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
