using UnityEngine;

public class Creature : Entity
{
    //these are mostly randomly set so that things don't crash. Each creature should set it to the correct value in their child class
    public float hunger = 0; //current hunger?
    public float energy = 2; //current health

    public float maxEnergy = 5; //current health cap
    protected float growthRate = 0.1f; //how much it grows per minute
    protected static float adultEnergy = 20; //max energypool it can grow to
    protected int adultSize = 1; //max size it can grow to

    protected float spawnSize = 0.1f; //size it starts as (since adult size is 1, its a percentage)
    protected float spawnRadius = 5; //how far away it can spawn offspring
    protected float minSpawnSpace = 1; //space needed for offspring to spawn (avoids crowding, needs to be smaller than spawnRadius)
    protected float minCMCubedPer = 10000; //limits max population according to the size of the tank (each creature needs this amount of cm^3 of water)
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected new void Awake()
    {
        base.Awake();
        initSize();

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    protected void initSize()
    {
        setMaturity(spawnSize);
        energy = maxEnergy;
    }


    /// <summary> scales up size and energy by the percentage passed in (wont exceed max size set) </summary>
    public void grow(float percentage)
    {
        if (maxEnergy + (adultEnergy * percentage) > adultEnergy)
        {
            setMaturity(1);
        }
        else if (maxEnergy + (adultEnergy * percentage) <= 0)
        {
            die();
            Debug.LogWarning("Shrunk to death");
        }
        else
        {
            setMaturity(percentage + maxEnergy/adultEnergy);
        }
    }
/// <summary>
/// set growth to this size, keeping size and energy proportional (unlike grow, which ADDS the percentage to the current size). Percentage cant be negative
/// </summary>
    public void setMaturity(float percentage){
        if(percentage <= 0) {Debug.LogWarning("maturity cannot be negative"); }
        setScaleTo(adultSize * percentage); 
        maxEnergy = percentage * adultEnergy;
    }

    /// <summary> make identical copy (for now, in a random position nearby. This is probably temporary). </summary>
    public void duplicate(Vector3 position) 
    {
        if (parentAquarium == null) { Debug.LogWarning("Could not find Aquarium parent"); return; }
        if (!parentAquarium.isInBounds(position)) {return; } //keep w/in aquarium

        parentAquarium.addEntity(this, position, transform.localRotation); //spawn nearby in same aquarium
        beingEaten(spawnSize*adultEnergy, false); //lose the same amount as the new creature spawned has
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
        if ((numTInTank > 0 )) { currCMCubedPerT = parentAquarium.volume() / numTInTank; }

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
            beingEaten(spawnSize * adultEnergy, false); //lose some energy to slow duplicate attempts
        }

    }

    /// <summary>
    /// energy -= amount. scaleDown if it is like algea and should become smaller. dont scaleDown if it is like a trilobite and should stay the same size at low energy
    /// </summary>
    /// <returns> amount of energy gained (will be amount if this has that amount of energy to give, or less if it was already low on energy) </returns>
    public float beingEaten(float amount, bool scaleDown = false)
    {
        if (amount < 0) { Debug.LogWarning("beingEaten() amount negative"); return 0; }

        if (energy - amount <= 0) //if the creature does not have enough energy to survive the eating
        {
            die();
            return energy;
        }

        if (!scaleDown) //if it does survive and should not shrink
        {
            energy -= amount;
        }
        else //if it does survive and should shrink
        {
            energy -= amount;
            grow(-amount/adultEnergy); //will never shrink to death
        }
        return amount;
    }

    public void eat(float amount)
    {
        if (amount < 0) { Debug.LogWarning("eat() amount negative"); return; }
        if(energy + amount > maxEnergy)
        {
            energy = maxEnergy;
        }
        else energy += amount;
    }


    //completely disable creature and dont let it interact with "real" creatures (for shop display, drag n drop preview etc)
    public override void initShopMode(bool asAdult = true, bool changeMaturity = true)
    {
        if (changeMaturity && asAdult) setMaturity(1);
        else if (changeMaturity && !asAdult) setMaturity(spawnSize);
        BoxCollider BC = GetComponent<BoxCollider>(); Rigidbody RB = GetComponent<Rigidbody>();
        if (BC) { BC.enabled = false; Destroy(BC); }
        if (RB) { Destroy(RB); } //this is the only way to turn off the RB for whatever reason

        this.enabled = false; //turn off Update() and Start()
        shopMode = true;

    }

}

