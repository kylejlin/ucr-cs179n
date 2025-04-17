using UnityEngine;

public class Creature : Entity
{
    public float hunger = 0;
    public float health = 2; //current health
    protected float maxHealth = 5; //current health cap
    protected float growthRate = 0.1f; //how much it grows per minute
    protected float adultHealth = 20; //max healthpool it can grow to
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
        if (maxHealth + (adultHealth * percentage) > adultHealth)
        {
            maxHealth = adultHealth;
            setScaleTo(adultSize);
        }
        else if (maxHealth + (adultHealth * percentage) <= 0)
        {
            die();
            Debug.LogWarning("Shrunk to death");
        }
        else
        {
            maxHealth += percentage * adultHealth;
            setScaleTo(maxHealth / adultHealth * adultSize); //keep size and maxHealth proportional

        }
    }

    /// <summary> make identical copy (for now, in a random position nearby. This is probably temporary). </summary>
    public void duplicate(Vector3 position) 
    {
        if (parentAquarium == null) { Debug.LogWarning("Could not find Aquarium parent"); return; }

        parentAquarium.addEntity(this, position, transform.localRotation); //spawn nearby in same aquarium
        beingEaten(2, true); //lose some health //this should be the same amount as the new creature spawned has, but for now is hard-coded
    }

    /// <summary> try to duplicate, but dont if there is a T too close to the attempted spawn location or too many of T in the aquarium as a whole. </summary>
    public void tryDuplicate<T>(float minSpace = 0, float maxDensityInTankPerUnitCubed = int.MaxValue) where T : Entity
    {
        Vector3 randVec = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)); //for now range is hardcoded to 5. In the future it should be a creature parameter 


    }

    /// <summary>
    /// health -= amount. scaleDown if it is like algea and should become smaller. dont scaleDown if it is like a trilobite and should stay the same size at low health
    /// </summary>
    /// <returns> amount of health gained (will be amount if this has that amount of health to give, or less if it was already low on health) </returns>
    public float beingEaten(float amount, bool scaleDown = false)
    {
        if (amount < 0) { Debug.LogWarning("beingEaten() amount negative"); return 0; }

        if (health - amount < 0) //if the creature does not have enough health to survive the eating
        {
            die();
            return health;
        }

        if (!scaleDown) //if it does survive and should not shrink
        {
            health -= amount;
        }
        else //if it does survive and should shrink
        {
            health -= amount;
            grow(-amount/adultHealth); //will never shrink to death
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
