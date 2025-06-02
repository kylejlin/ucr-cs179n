using UnityEngine;

public class ImmobileCreature : Creature
{
    [SerializeField] protected float eatRate = 5f; //how much it eats (through photosynthesis or filter feeding or w/e) per minute
    [SerializeField] protected float growthCooldown = 60f; //how often it grows (by growthrate)
    protected new void Awake()
    {

        base.Awake();
        initSize();

    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void automaticGrowth(float deltaTime)
    {
        count += deltaTime;
        if (count > growthCooldown) 
        {
            grow(growthRate);
            count = 0;
            if ((getScale() == adultSize) && (energy == adultEnergy)) { tryDuplicate<Algea>(minSpawnSpace, minCMCubedPer); }
        }
        eat(eatRate / 5 * deltaTime);
    }


}
