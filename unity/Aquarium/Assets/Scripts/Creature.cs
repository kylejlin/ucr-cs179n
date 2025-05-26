using Unity.VisualScripting;
using UnityEngine;

public class Creature : Entity
{
    //for each individual type of creature these should be set in the prefab editor 
    public float energy = 2; //current health

    [SerializeField] public float maxEnergy = 5; //current health cap
    [SerializeField] protected float growthRate = 0.1f; //how much it grows per minute
    [SerializeField] protected float adultEnergy = 20; //max energypool it can grow to
    [SerializeField] protected int adultSize = 1; //max size it can grow to
    [SerializeField] public float metabolismRate = 1;

    [SerializeField] protected float spawnSize = 0.1f; //size it starts as (since adult size is 1, its a percentage)
    [SerializeField] protected float spawnRadius = 5; //how far away it can spawn offspring
    [SerializeField] protected float minSpawnSpace = 1; //radius of space needed around offspring to spawn (avoids crowding, needs to be smaller than spawnRadius)
    [SerializeField] protected float minCMCubedPer = 10000; //limits max population according to the size of the tank (each creature needs this amount of cm^3 of water)
    [SerializeField] protected bool scaleDownWhenEaten = false; //should the creature scale down when predated upon (ex. algea) or not (ex. animals) //set in editor 
    [SerializeField] public bool mustBeKilledToBeEaten = false; //does this creature need to die to be predated upon (ex. animals) for not (ex. algea)
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
    public virtual void grow(float percentage)
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
            setMaturity(percentage + maxEnergy / adultEnergy);
        }
    }
    /// <summary>
    /// set growth to this size, keeping size and energy proportional (unlike grow, which ADDS the percentage to the current size). Percentage cant be negative
    /// </summary>
    public virtual void setMaturity(float percentage)
    {
        if (percentage <= 0) { Debug.LogWarning("maturity cannot be negative"); }
        setScaleTo(adultSize * percentage);
        maxEnergy = percentage * adultEnergy;
    }
    public float getMaturity()
    {
        return transform.localScale.x;
    }

    /// <summary> make identical copy (for now, in a random position nearby. This is probably temporary). </summary>
    public virtual void duplicate(Vector3 position)
    {
        if (parentAquarium == null) { Debug.LogWarning("Could not find Aquarium parent"); return; }
        if (!parentAquarium.isInBounds(position)) { return; } //keep w/in aquarium

        bool isOutlinedRN = isOutlined();
        setOutline(false); //have to do this else the outline materials will get duplicated onto the child and I cant find a better way to stop this

        parentAquarium.addEntity(this, position, transform.localRotation); //spawn nearby in same aquarium
        loseEnergy(spawnSize * adultEnergy, false); //lose the same amount as the new creature spawned has

        setOutline(isOutlinedRN);
    }

    /// <summary> try to duplicate, but dont if there is a T too close to the attempted spawn location or too many of T in the aquarium as a whole. </summary>
    public virtual void tryDuplicate<T>(float minSpace = 0, float minCMCubedPerT = 0) where T : Entity
    {
        //this will also break if this entity is not a direct child of aquarium
        Vector3 randVecNearby = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius)) + transform.localPosition;

        T closestT = parentAquarium.FindClosest<T>(randVecNearby);//TODO: inconsistent with mobileCreature duplicate
        float closestTSqrDist = Mathf.Infinity;
        if (closestT != default(T)) { closestTSqrDist = getSqrDistBw(closestT.transform.localPosition, randVecNearby); }

        int numTInTank = parentAquarium.getAllOfType(this).Count;
        float currCMCubedPerT = Mathf.Infinity;
        if ((numTInTank > 0)) { currCMCubedPerT = parentAquarium.volume() / numTInTank; }

        //Debug.Log("currPos "+ transform.localPosition);
        //Debug.Log("nrarbyPost "+ randVecNearby);
        //Debug.Log("closest "+ closestTSqrDist);
        //Debug.Log("curr units per T "+ currUnitsCubedPerT);
        if ((closestTSqrDist > minSpace * minSpace) && (minCMCubedPerT < currCMCubedPerT))
        {
            duplicate(randVecNearby);
        }
        else
        {
            loseEnergy(spawnSize * adultEnergy, false); //lose some energy to slow duplicate attempts
        }

    }

    /// <summary>
    /// energy -= amount. scaleDown if it is like algea and should become smaller. dont scaleDown if it is like a trilobite and should stay the same size at low energy
    /// </summary>
    /// <returns> amount of energy gained (will be amount if this has that amount of energy to give, or less if it was already low on energy) </returns>
    public float beingEaten(float amount)
    {
        return loseEnergy(amount, scaleDownWhenEaten);
    }

    public float loseEnergy(float amount, bool shouldShrink)
    {
        if (amount < 0) { Debug.LogWarning("loseEnergy() amount negative"); return 0; }

        if (energy - amount <= 0) //if the creature does not have enough energy to survive the eating
        {
            die();
            return energy;
        }

        if (!shouldShrink) //if it does survive and should not shrink
        {
            energy -= amount;
        }
        else //if it does survive and should shrink
        {
            energy -= amount;
            grow(-amount / adultEnergy); //will never shrink to death because of earlier check
        }
        return amount;
    }

    public void eat(float amount)
    {
        if (amount < 0) { Debug.LogWarning("eat() amount negative"); return; }
        if (energy + amount > maxEnergy)
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

        this.enabled = false;  //no update()
        shopMode = true;
        enableAllColliders(false); //dont mess w collisions or raycasts etc

        Rigidbody RB = GetComponent<Rigidbody>();
        if (RB) RB.isKinematic = true; //no physics please

    }

    public override float calcMoneyBonus()
    {
        float currMoneyBonus = getHappiness();

        switch (GetRarity())
        {
            case Rarity.Common:
                return 1 + currMoneyBonus;
            case Rarity.Rare:
                return 3 + currMoneyBonus;
            case Rarity.Epic:
                return 7 + currMoneyBonus;
            default:
                return 1 + currMoneyBonus;
        }
    }

    public override float getHappiness()
    {
        float happiness = energy / maxEnergy * 5;
        if (parentAquarium.getAllOfType(this).Count > 1) //if creature is not the last of its kind
        {
            happiness += 3;
        }
        return happiness;
    }
    
}

