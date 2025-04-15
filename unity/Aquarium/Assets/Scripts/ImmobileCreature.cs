using UnityEngine;

public class ImmobileCreature : Creature
{
    [HideInInspector]
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        size = 0.1f;
        setScaleTo(size);
        health = 2;
        maxHealth = 10;
        growthRate = 0.4f;
        count += Random.Range(-5, 0);
    }

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;
        if (count > 5)
        {
            grow(growthRate);
            count = 0;
            if ((size == adultSize) && (health == adultHealth)) { duplicate(); print("boom"); }
        }
        eat(5 * Time.deltaTime);
    }

}
