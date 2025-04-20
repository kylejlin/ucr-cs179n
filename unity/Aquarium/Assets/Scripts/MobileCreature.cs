using UnityEngine;

public class MobileCreature : Creature
{
    public bool canSwim = false;
    public int consumeRate;
    public float speed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected new void Start()
    {
        base.Start(); //call Creature Start()
        name = "Trilobite "+ entityName;
        growthRate = 0.1f; 
        adultHealth = 20; 

        spawnSize = 1f; //for demo, have them spawn fully grown
        spawnRadius = 5;
        minSpawnSpace = 1; 
        minCMCubedPer = 2000;
        initSize();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
