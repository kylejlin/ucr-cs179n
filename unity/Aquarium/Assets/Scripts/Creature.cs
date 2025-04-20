using UnityEngine;

public class Creature : Entity
{
    //these are mostly randomly set so that things don't crash. Each creature should set it to the correct value in their child class
    public float hunger = 0; //current hunger?
    public float health = 2; //current health

    public float maxHealth = 5; //current health cap
    protected float growthRate = 0.1f; //how much it grows per minute
    protected float adultHealth = 20; //max healthpool it can grow to
    protected int adultSize = 1; //max size it can grow to

    protected float spawnSize = 0.1f; //size it starts as (since adult size is 1, its a percentage)
    protected float spawnRadius = 5; //how far away it can spawn offspring
    protected float minSpawnSpace = 1; //space needed for offspring to spawn (avoids crowding, needs to be smaller than spawnRadius)
    protected float minCMCubedPer = 10000; //limits max population according to the size of the tank (each creature needs this amount of cm^3 of water)
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected new void Start()
    {
        base.Start();
        initSize();

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    protected void initSize()
    {
        setMaturity(spawnSize);
        health = maxHealth;
    }

    /// <summary> scales up size and health by the percentage passed in (wont exceed max size set) </summary>
    public void grow(float percentage)
    {
        if (maxHealth + (adultHealth * percentage) > adultHealth)
        {
            setMaturity(1);
        }
        else if (maxHealth + (adultHealth * percentage) <= 0)
        {
            die();
            Debug.LogWarning("Shrunk to death");
        }
        else
        {
            setMaturity(percentage + maxHealth/adultHealth);
        }
    }
/// <summary>
/// set growth to this size, keeping size and health proportional (unlike grow, which ADDS the percentage to the current size). Percentage cant be negative
/// </summary>
    public void setMaturity(float percentage){
        if(percentage <= 0) {Debug.LogWarning("maturity cannot be negative"); }
        setScaleTo(adultSize * percentage); 
        maxHealth = percentage * adultHealth;
    }

    /// <summary> make identical copy (for now, in a random position nearby. This is probably temporary). </summary>
    public void duplicate(Vector3 position) 
    {
        if (parentAquarium == null) { Debug.LogWarning("Could not find Aquarium parent"); return; }
        if (!parentAquarium.isInBounds(position)) {return; } //keep w/in aquarium

        parentAquarium.addEntity(this, position, transform.localRotation); //spawn nearby in same aquarium
        beingEaten(spawnSize*adultHealth, false); //lose the same amount as the new creature spawned has
    }

    /// <summary> try to duplicate, but dont if there is a T too close to the attempted spawn location or too many of T in the aquarium as a whole. </summary>
    public void tryDuplicate<T>(float minSpace = 0, float minCMCubedPerT = 0) where T : Entity
    {
        Vector3 randVecNearby = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius)) + transform.localPosition; 
        
        T closestT = FindClosest<T>(randVecNearby); 
        float closestTSqrDist =  Mathf.Infinity; 
        if (closestT != default(T)) {closestTSqrDist = getSqrDistBw(FindClosest<T>(randVecNearby).transform.localPosition, randVecNearby); } 
        
        int numTInTank = getAllOfType<T>().Length;
        float currCMCubedPerT = Mathf.Infinity;
        if (numTInTank > 0 ) { currCMCubedPerT = parentAquarium.volume() / getAllOfType<T>().Length; }

        //Debug.Log("currPos "+ transform.localPosition);
        //Debug.Log("nrarbyPost "+ randVecNearby);
        //Debug.Log("closest "+ closestTSqrDist);
        //Debug.Log("curr units per T "+ currUnitsCubedPerT);
        if ((closestTSqrDist > minSpace*minSpace) && (minCMCubedPerT < currCMCubedPerT))
        {
            duplicate(randVecNearby);
        }
        else
        {
            beingEaten(spawnSize * adultHealth, false); //lose some health to slow duplicate attempts
        }

    }

    /// <summary>
    /// health -= amount. scaleDown if it is like algea and should become smaller. dont scaleDown if it is like a trilobite and should stay the same size at low health
    /// </summary>
    /// <returns> amount of health gained (will be amount if this has that amount of health to give, or less if it was already low on health) </returns>
    public float beingEaten(float amount, bool scaleDown = false)
    {
        if (amount < 0) { Debug.LogWarning("beingEaten() amount negative"); return 0; }

        if (health - amount <= 0) //if the creature does not have enough health to survive the eating
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

    protected void initDisplayMode(){
        setMaturity(1);
        this.enabled = false; //turn off Update()
    }

}
