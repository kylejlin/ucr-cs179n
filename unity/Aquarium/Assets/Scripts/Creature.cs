using UnityEngine;

public class Creature : Entity
{
    public float hunger = 0;
    protected int maxHealth = 10; //Maximum health that this creature can grow to in its life
    protected int maxSize = 1; //Maximum szie that this creature can grow to in its life
    public float currFullHealth = 10; //current health cap (if its the same as maxHealth then its fully grown)
    public float currHealth = 10; //current health
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void grow(float percentage)
    {
        float perc = Mathf.Clamp(percentage, 0, 1); //limit to between 0 and 1
        currFullHealth += maxHealth * perc;
        if (currFullHealth > maxHealth) { currFullHealth = maxHealth; }

        float newScale = transform.localScale.x + maxSize * perc;
        if (newScale > maxSize) { newScale = maxSize; }

        setScaleTo(newScale);
    }
}
