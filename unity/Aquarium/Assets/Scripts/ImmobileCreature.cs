using UnityEngine;

public class ImmobileCreature : Creature
{
    [HideInInspector]
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void beingEaten(int amount)
    {
        if (amount < 0) { Debug.LogWarning("beingEaten() amount negative"); return; }

        health -= amount;
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }
        setScaleTo((float)health / maxHealth);
    }

}
