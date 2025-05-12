using UnityEngine;

public class ImmobileCreature : Creature
{
    [HideInInspector]
    protected float eatRate = 5f; //how much it eats (through photosynthesis or filter feeding or w/e) per minute
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected new void Awake()
    {
        
        base.Awake();
        name = "Algea "+ entityName;
        growthRate = 0.1f; 
        adultEnergy = 40; 

        spawnSize = 0.1f;
        spawnRadius = 20;
        minSpawnSpace = 5; 
        minCMCubedPer = 10000;
        initSize();


    }

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;
        if (count > 2) //sped up for demo
        {
            grow(growthRate);
            count = 0;
            if ((getScale() == adultSize) && (energy == adultEnergy)) { tryDuplicate<ImmobileCreature>(minSpawnSpace, minCMCubedPer); }
        }
        eat(eatRate/5 * Time.deltaTime);
    }



}
