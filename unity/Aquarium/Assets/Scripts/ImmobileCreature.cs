using UnityEngine;

public class ImmobileCreature : Creature
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getEaten(float amount) //TODO: needs to check for death
    {
        currFullHealth -= amount;
        currHealth -= amount;
        setScaleTo(transform.localScale.x - amount / maxHealth * maxSize);
    }
}
