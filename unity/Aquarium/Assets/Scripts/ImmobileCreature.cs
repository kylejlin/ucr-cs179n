using UnityEngine;

public class ImmobileCreature : Entity
{
    [HideInInspector]
    public int health;
    public int maxHealth;
    public int growthRate;
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
        health -= amount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void grow(int amount)
    {
        health += amount;
        if (health > 100) // todo: change to maxHealth
        {
            health = 100; // todo: change to maxHealth
        }
    }
}
