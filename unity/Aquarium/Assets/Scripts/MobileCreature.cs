using UnityEngine;

public class MobileCreature : Creature
{
    public bool canSwim = false;
    public int consumeRate;
    public float speed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        MobileCreature friend = FindClosest<MobileCreature>();
        if(friend == default(MobileCreature)) Debug.Log("None found");
        else Debug.Log("nearest trilo pos:"+friend.transform.localPosition.x);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
