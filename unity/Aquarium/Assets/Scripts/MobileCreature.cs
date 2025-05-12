using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum BehaviorState
{
    Idle,
    Hunting,
    Dying,
}

public class MobileCreature : Creature
{   
    
    public bool canSwim = false;
    public float consumeRate = 20;
    public float speed = 1;

    public float huntingEnergyThreshold = 30;
    public static float metabolismRate = 1;
    public static float maxEatingDistance = 7;

    public BehaviorState state = BehaviorState.Idle;

    private Rigidbody mobileCreatureRB;

    protected double breedingCooldown = 10;
    public GameObject childPrefab; //used to instantiate new prefab for child 

    /// <summary> Set default values for trilobite bought from store. </summary>
    private void setValues(bool demo)
    {
        if(demo){
            name = "Trilobite " + entityName;
            growthRate = 0.1f;
            adultEnergy = 40;
            breedingCooldown = 50; //change 

            spawnSize = .5f; 
            energy = maxEnergy = spawnSize*adultEnergy;
            huntingEnergyThreshold = .75f * maxEnergy;
            consumeRate = .5f * maxEnergy; //changed

            spawnRadius = 20;
            minSpawnSpace = 5;
            minCMCubedPer = 10000; //changed
        }else{
            name = "Trilobite " + entityName;
            growthRate = 0.1f;
            adultEnergy = 40;
            breedingCooldown = 100; //change 

            spawnSize = .5f; 
            energy = maxEnergy = spawnSize*adultEnergy;
            huntingEnergyThreshold = .75f * maxEnergy;
            consumeRate = .5f * maxEnergy; //changed

            spawnRadius = 5;
            minSpawnSpace = 1;
            minCMCubedPer = 200; //changed
        }
    }

    public void setChildValues(MobileCreature parent1, MobileCreature parent2)
    {
        speed = parent1.speed + parent2.speed;
        count = 0;
        //color
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected new void Awake()
    {
        base.Awake(); //call Creature Start()
        mobileCreatureRB = GetComponent<Rigidbody>();
        setValues(true);

        initSize();
    }

    // FixedUpdate is called at fixed time intervals
    void FixedUpdate()
    {
        energy -= Time.deltaTime * metabolismRate/2;

        //if the creature is an adult, high energy, and past the breedingCooldown period, try breeding
        if(getScale() == adultSize){
            count += Time.deltaTime;
            if((energy > adultEnergy*.75f) && (count > breedingCooldown)){
                //print("Trying to duplicate");
                tryDuplicate<MobileCreature>(minSpawnSpace, minCMCubedPer); 
            }
        }
        
        if (energy <= 0)
        {
            // The creature starved to death. 
            state = BehaviorState.Dying;
        }
        else if (energy <= huntingEnergyThreshold)
        {
            state = BehaviorState.Hunting;
        }
        else
        {
            state = BehaviorState.Idle;
        }


        if (state == BehaviorState.Idle)
        {
            UpdateIdle();
        }
        else if (state == BehaviorState.Hunting)
        {
            UpdateHunting();
        }
        else if (state == BehaviorState.Dying)
        {
            UpdateDying();
        }
    }

    void UpdateIdle()
    {
        Vector3 vec = new Vector3(0, 0, 1); //move forward
        move(vec);
        if (transform.localRotation.eulerAngles.x > 45)
        { //if creature pointed downwards, rotate upwards
            // Vector3 angVec = new Vector3(30, 0, 0); 
            // rotate(angVec);
        }
        else
        {
            Vector3 angVec = new Vector3(0, 10, 0); //rotate a little bit around axes
            rotate(angVec);
        }

    }

    void UpdateDying()
    {
        // TODO
        // Vector3 angVec = new Vector3(0, 0, 180); //rotate a little bit around axes
        // rotate(angVec);
        die();
    }

    void UpdateHunting()
    {
        // I can't find the algae class, so for now,
        // I'm just targeting the closest entity.
        ImmobileCreature closest = FindClosest<ImmobileCreature>();

        if (closest == null)
        {
            // Nothing to eat.
            return;
        }

        Vector3 delta = closest.transform.position - transform.position;
        float distance = delta.magnitude;

        if (distance <= maxEatingDistance)
        {
            //Eat based on consumeRate 
            eat(closest.beingEaten(consumeRate));
            grow(growthRate);
            return;
        }

        Vector3 displacement = delta.normalized;
        float k = speed*3 * Time.deltaTime;
        displacement.Scale(new Vector3(k, k, k));
        transform.position += displacement;
        rotateTowards(delta);
    }

    //Takes in Vector3 velocity to move mobileCreature
    private void move(Vector3 velocity)
    {
        if (shopMode) { Debug.LogWarning("Can't move in shop mode"); return; }
        //using rigidbody.MovePosition() will make transitioning to the new position smoother if interpolation is enabled
        //MovePosition(currentPosition + displacement)
        mobileCreatureRB.MovePosition(mobileCreatureRB.position + mobileCreatureRB.rotation * velocity * speed * Time.fixedDeltaTime);
    }

    //Takes in Vector3 angularVelocity to rotate mobileCreature
    private void rotate(Vector3 angularVelocity)
    {
        if (shopMode) { Debug.LogWarning("Can't rotate in shop mode"); return; }
        //angularVelocity tells how many degrees to rotate in each axis
        Quaternion deltaRotation = Quaternion.Euler(angularVelocity * Time.fixedDeltaTime);
        mobileCreatureRB.MoveRotation(mobileCreatureRB.rotation * deltaRotation);
    }

    //Takes in Vector3 angularVelocity to rotate mobileCreature towards direction of vector
    private void rotateTowards(Vector3 angularVelocity)
    {
        if (shopMode) { Debug.LogWarning("Can't rotate in shop mode"); return; }
        //rotates creature with respect to front of creature (head points towards rotation)
        mobileCreatureRB.MoveRotation(Quaternion.LookRotation(angularVelocity, Vector3.forward));
    }

    /// <summary> scales up size and energy by the percentage passed in (wont exceed max size set) </summary>
    public override void grow(float percentage)
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
    /// override creature setMaturity() function to add mobileCreature values that should scale
    /// </summary>
    public override void setMaturity(float percentage){
        if(percentage <= 0) {Debug.LogWarning("maturity cannot be negative"); }
        setScaleTo(adultSize * percentage); 
        maxEnergy = percentage * adultEnergy;
        huntingEnergyThreshold = .75f * maxEnergy;
        consumeRate = .5f * maxEnergy;
    }

    /// <summary> make identical copy (for now, in a random position nearby. This is probably temporary). </summary>
    public void duplicate<T>(Vector3 position, T partner) where T : MobileCreature
    {
        if (parentAquarium == null) { parentAquarium.setBreedingMutex(false); Debug.LogWarning("Could not find Aquarium parent"); return; }
        if (partner == null) { parentAquarium.setBreedingMutex(false); Debug.LogWarning("Could not find breeding partner"); return; }        
        if (!parentAquarium.isInBounds(position)) {parentAquarium.setBreedingMutex(false); return; } //keep w/in aquarium
        
        if(partner != null){
            
        }
        print("Instantiating");
        
        MobileCreature child = (MobileCreature) parentAquarium.addEntity(childPrefab.GetComponent<Entity>(), position, transform.localRotation); //spawn nearby in same aquarium
        child.setChildValues(this,partner);
        count = 0;
        partner.count = 0;
        parentAquarium.setBreedingMutex(false);
    }

    /// <summary> try to duplicate, but dont if there is a T too close to the attempted spawn location or too many of T in the aquarium as a whole. </summary>
    public override void tryDuplicate<T>(float minSpace = 0, float minCMCubedPerT = 0) //T constraint removed 
    {
        
        Vector3 randVecNearby = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius)) + transform.localPosition; 
        
        MobileCreature closestT = FindClosest<MobileCreature>(randVecNearby); 
        float closestTSqrDist =  Mathf.Infinity; 
        if (closestT != default(T)) {closestTSqrDist = getSqrDistBw(FindClosest<T>(randVecNearby).transform.localPosition, randVecNearby); } 
        
        int numTInTank = getAllOfType<T>().Length;
        float currCMCubedPerT = Mathf.Infinity;
        if ((numTInTank > 0 )) { currCMCubedPerT = parentAquarium.volume() / numTInTank; }
        

        //Debug.Log("currPos "+ transform.localPosition);
        //Debug.Log("nrarbyPost "+ randVecNearby);
        //Debug.Log("closest "+ closestTSqrDist);
        //Debug.Log("curr units per T "+ currUnitsCubedPerT);
        if ((closestTSqrDist > minSpace*minSpace) && (minCMCubedPerT < currCMCubedPerT) && !(parentAquarium.getBreedingMutex()))
        {
            print("Call duplicate");
            parentAquarium.setBreedingMutex(true);
            duplicate(randVecNearby,findPartner<MobileCreature>(closestT));
        }
    }

    /// <summary> Find closest available creature of same type who is also ready to breed </summary>
    public T findPartner<T>(T closestT) where T:MobileCreature {
        if(closestT.readyToBreed() && this.GetType().Equals(closestT.GetType())){
            return closestT;
        }
        return null;
    }

    /// <summary>
    /// if the creature is an adult, high energy, and past the breedingCooldown period
    /// </summary>
    public bool readyToBreed(){
        if(getScale() == adultSize){
            if((energy > adultEnergy*.75) && (count > breedingCooldown)){
                return true;
            }
        }
        return false;
    }

}