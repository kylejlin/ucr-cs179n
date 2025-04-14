using UnityEngine;

public class Creature : Entity
{
    public float hunger = 0;
    public int health = 2; //current health
    protected int maxHealth = 10; //current health cap
    protected int growthRate = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void grow(int amount)
    {
        Debug.Log("ucrrHealth: " + health + "max" + maxHealth + " scale to " + health / maxHealth);
        if (amount<0) { Debug.LogWarning("grow() amount negative"); return; }
        health += amount;
        if (health > maxHealth) 
        {
            health = maxHealth; 
        }
        setScaleTo((float)health/maxHealth);
    }
}
