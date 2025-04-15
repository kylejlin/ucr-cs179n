using UnityEngine;

public class Creature : Entity
{
    public float hunger = 0;
    public float health = 2; //current health
    protected float maxHealth = 5; //current health cap
    protected float growthRate = 0.1f; //how much it grows per minute
    protected float adultHealth = 20; //max healthpool it can grow to
    protected float size = 0.1f; //current size
    protected int adultSize = 1; //max size it can grow to
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary> scales up size and health by the percentage passed in (wont exceed max size set) </summary>
    public void grow(float percentage)
    {
        if (percentage<0) { Debug.LogWarning("grow() percentage negative"); return; }
        if (size + adultSize*percentage > adultSize)
        {
            size = adultSize;
            maxHealth = adultHealth;
            setScaleTo(adultSize);
        }
        else
        {
            size += percentage * adultSize;
            maxHealth += percentage * adultHealth;
            setScaleTo(size);

        }
    }

    public void duplicate()
    {
        if (parentAquarium == null) { Debug.LogWarning("Could not find Aquarium parent"); return; }

        Vector3 randVec = new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
        parentAquarium.addEntity(this, randVec + transform.localPosition, transform.localRotation); //spawn nearby in same aquarium
        beingEaten(2, true); //lose some health 
    }

    /// <summary>
    /// health -= amount. scaleDown if it is like algea and should become smaller. dont scaleDown if it is like a trilobite and should stay the same size at low health
    /// </summary>
    /// <returns> amount of health gained (will be amount if this has that amount of health to give, or less if it was already low on health) </returns>
    public float beingEaten(float amount, bool scaleDown)
    {
        if (amount < 0) { Debug.LogWarning("beingEaten() amount negative"); return 0; }

        if (health - amount < 0)
        {
            die();
            return health;
        }
        health -= amount;
        if (scaleDown)
        {
            size = health / maxHealth;
            setScaleTo(size);
            maxHealth -= amount;
        }
        return amount;
    }

    public void eat(float amount)
    {
        if (amount < 0) { Debug.LogWarning("eat() amount negative"); return; }
        if(health + amount > maxHealth)
        {
            health = maxHealth;
        }
        else health += amount;
    }
}
