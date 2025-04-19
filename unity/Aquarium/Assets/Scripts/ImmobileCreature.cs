using UnityEngine;

public class ImmobileCreature : Creature
{
    [HideInInspector]
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        growthRate = 0.5f;
        setScaleTo(spawnSize);
        name = "Algea "+ entityName;
    }

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;
        if (count > 5)
        {
            grow(growthRate);
            count = 0;
            if ((getScale() == adultSize) && (health == adultHealth)) { tryDuplicate<ImmobileCreature>(5, 20000); print("boom"); }
        }
        eat(1 * Time.deltaTime);
    }



}
