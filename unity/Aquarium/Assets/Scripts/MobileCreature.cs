using System.Collections.Generic;
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
    public float consumeRate = 20f;
    public float speed = 1;

    public float huntingEnergyThreshold = 30;
    public static float maxEatingDistance = 1;

    public BehaviorState state = BehaviorState.Idle;

    private Rigidbody mobileCreatureRB;
    [SerializeField] protected Animator animator; 

    [SerializeField] protected double breedingCooldown = 10;
    [SerializeField] protected double predateCooldown = 0.5f; // how long between damage dealing events. can think of it like its dps. bigger values will require more "fighting"
    private float predateCount = 0f; //delta time tracking for ^

    public GameObject childPrefab; //used to instantiate new prefab for child 

    public float navigationTemperature = 0;

    public Vector3 previousTargetPosition = Vector3.zero;

    public bool isIgnoringNav = false;

    public float ignoreNavTemperatureThreshold = 3f;

    /// <summary> Set default values for trilobite bought from store. </summary>
    protected new void Awake()    {
        base.Awake(); //call Creature Start()
        mobileCreatureRB = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        initSize();

    }
    /// <summary> Set default values for trilobite bought from store. </summary>


    public void setChildValues(MobileCreature parent1, MobileCreature parent2)
    {
        speed = (parent1.speed + parent2.speed)/2;
        count = 0;
        //color
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
    }

    // FixedUpdate is called at fixed time intervals
    void FixedUpdate()
    {
        updateFSM();
    }

    protected virtual void updateFSM()
    {
        energy -= Time.deltaTime * metabolismRate / 2;

        //if the creature is an adult, high energy, and past the breedingCooldown period, try breeding
        if (getScale() == adultSize)
        {
            count += Time.deltaTime;
            if ((energy > adultEnergy * .75f) && (count > breedingCooldown))
            {
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
            if (animator) animator.SetTrigger("hunt");
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

    protected virtual void UpdateIdle()
    {
        Vector3 vec = new Vector3(0, 0, 1); //move forward
        move(vec);
        float lookAheadDist = 10f * getMaturity(); //would be better for this to be based on collider size and not hard coded
        if (!parentAquarium.checkVoxelInFrontForObstacle(getAllCollidersBoundingBox().center, transform.TransformVector(new Vector3(0, 0, 1)), lookAheadDist))
        {
            //if somethings in the way, rotate
            Vector3 angVec = new Vector3(10, 50, 1);
            rotate(angVec);
        }


        if(!canSwim) mobileCreatureRB.AddForce(0, -8, 0); //stay on the bottom
        // todo: make it stay upright
        // if(transform.localEulerAngles.z > 45) mobileCreatureRB.AddRelativeTorque(0, 0, 1); //keep upright
        // if(transform.localEulerAngles.z < -45) mobileCreatureRB.AddRelativeTorque(0, 0, -1); //keep upright




        // rotate(new Vector3(10, 10, -transform.localEulerAngles.z));
        // print(transform.localEulerAngles.z);
        // mobileCreatureRB.MoveRotation(mobileCreatureRB.rotation * Quaternion.Euler(new Vector3(0,0,-transform.localRotation.z*100) * Time.fixedDeltaTime));

    }

    protected virtual void UpdateDying()
    {
        // TODO
        // Vector3 angVec = new Vector3(0, 0, 180); //rotate a little bit around axes
        // rotate(angVec);
        die();
    }

    protected virtual void UpdateHunting()
    {
        // You can toggle this to test the navigation system.
        bool USE_NAV = true;

        if (USE_NAV)
        {
            UpdateHuntingWithNav();
        }
        else
        {
            UpdateHuntingWithoutNav();
        }
    }

    void UpdateHuntingWithNav()
    {
        if (isIgnoringNav)
        {
            UpdateHuntingWithIgnoredNav();
            return;
        }

        // I can't find the algae class, so for now,
        // I'm just targeting the closest entity.
        ImmobileCreature closest = parentAquarium.FindClosest<ImmobileCreature>(this);

        if (closest == null)
        {
            UpdateIdle();
            return;
        }

        Bounds bounds = getAllCollidersBoundingBox(); //I want this in future code so moving it out of the {}
        // Eat the prey if it's close enough.
        {

            float simpleDistance = (closest.transform.position - transform.position).magnitude;

            Bounds preyBounds = closest.getAllCollidersBoundingBox();
            Vector3[] corners = GetCorners(bounds);
            Vector3[] preyCorners = GetCorners(preyBounds);
            float minDistance = float.MaxValue;
            foreach (var a in corners)
            {
                foreach (var b in preyCorners)
                {
                    float dist = Vector3.Distance(a, b);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                    }
                }
            }

            // We can eat the prey if either:
            //
            // 1. The prey is within the maximum eating distance,
            //    when we calculate distance using the default position reference point.
            // 2. The prey is within the maximum eating distance,
            //    when we calculate distance using the closest corner-to-corner distance.
            // 3. We're _inside_ the prey.
            //    Note that this assumes that the prey has a collider.
            //    If it doesn't, this code will crash.

            bool canEat = simpleDistance <= maxEatingDistance || minDistance <= maxEatingDistance || bounds.Intersects(preyBounds);

            if (canEat)
            {
                //Eat based on consumeRate 
                eat(closest.beingEaten(consumeRate));
                grow(growthRate);
                return;
            }
        }

        // ...Otherwise, move towards the scent of food.
        {
            Vector3 targetPositionInWorldCoords = getTargetPositionInWorldCoords();
            if (previousTargetPosition != targetPositionInWorldCoords)
            {
                previousTargetPosition = targetPositionInWorldCoords;
                navigationTemperature = 0;
            }
            else
            {
                // You can tweak the coefficient to change how quickly the creature
                // gets impatient and starts ignoring the navigation system.
                navigationTemperature += 1.0f * Time.deltaTime;

                if (navigationTemperature >= ignoreNavTemperatureThreshold)
                {
                    // You can tweak this value to change how long the creature ignores the navigation system.
                    navigationTemperature = 7.0f;

                    isIgnoringNav = true;
                    return;
                }
            }

            if (bounds.Contains(targetPositionInWorldCoords)) return; //prevents flailing around voxel center point. If its already on the target just stop moving

            Vector3 delta = targetPositionInWorldCoords - transform.position;

            Vector3 displacement = delta.normalized * Time.deltaTime * speed;
            // float k = speed * 3 * Time.deltaTime;
            // displacement.Scale(new Vector3(k, k, k));
            transform.position += displacement;
            rotateTowards(delta);
        }
    }

    static Vector3[] GetCorners(Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        return new Vector3[]
        {
            center + new Vector3(+extents.x, +extents.y, +extents.z),
            center + new Vector3(+extents.x, +extents.y, -extents.z),
            center + new Vector3(+extents.x, -extents.y, +extents.z),
            center + new Vector3(+extents.x, -extents.y, -extents.z),
            center + new Vector3(-extents.x, +extents.y, +extents.z),
            center + new Vector3(-extents.x, +extents.y, -extents.z),
            center + new Vector3(-extents.x, -extents.y, +extents.z),
            center + new Vector3(-extents.x, -extents.y, -extents.z),
        };
    }

    void UpdateHuntingWithIgnoredNav()
    {
        if (navigationTemperature <= 0f)
        {
            isIgnoringNav = false;
            navigationTemperature = 0f;
            return;
        }

        // You can tweak the coefficient to change how quickly the creature
        // goes back to using the navigation system.
        navigationTemperature -= 1.0f * Time.deltaTime;

        // When we're ignoring the navigation system,
        // we just move around randomly.
        // For now, I'll use the `UpdateIdle` as a reasonable default.
        // If you want to customize this behavior,
        // feel free to replace this with your own logic.
        UpdateIdle();
    }

    public Vector3Int getTargetPositionInVoxelCoords()
    {
        Vector3 positionInAquariumCoords = getPositionInAquariumCoords();
        return parentAquarium.getBestNeighborCoordsInVoxelCoords(positionInAquariumCoords);
    }

    Vector3 getTargetPositionInWorldCoords()
    {
        Vector3 positionInAquariumCoords = getPositionInAquariumCoords();
        Vector3 targetPositionInAquariumCoords = parentAquarium.getBestNeighborCoordsInAquariumCoords(positionInAquariumCoords);
        return parentAquarium.transformAquariumCoordsToWorldCoords(targetPositionInAquariumCoords);
    }

    // This is the old hunting algorithm.
    void UpdateHuntingWithoutNav()
    {
        // I can't find the algae class, so for now,
        // I'm just targeting the closest entity.
        ImmobileCreature closest = parentAquarium.FindClosest<ImmobileCreature>(this);

        if (closest == null)
        {
            // Nothing to eat.
            return;
        }

        Vector3 delta = closest.transform.position - transform.position;
        float distance = delta.magnitude;

        if (distance <= maxEatingDistance * getMaturity() +2)
        {
            //Eat based on consumeRate 
            if (animator) animator.SetTrigger("eat");
            predate(closest);
            return;
        }

        move(delta,true);
    }



    //Takes in Vector3 velocity to move mobileCreature, needs to have parameter true when creature is hunting
    protected void move(Vector3 velocity, bool hunting = false)
    {
        if (shopMode) { Debug.LogWarning("Can't move in shop mode"); return; }
        //using rigidbody.MovePosition() will make transitioning to the new position smoother if interpolation is enabled
        //MovePosition(currentPosition + displacement)
        if (hunting)
        {
            mobileCreatureRB.MovePosition(mobileCreatureRB.position + velocity * (speed / 2) * Time.fixedDeltaTime);
            rotateTowards(velocity);
        }
        else
        {
            mobileCreatureRB.MovePosition(mobileCreatureRB.position + mobileCreatureRB.rotation * velocity * speed * Time.fixedDeltaTime);
        }
    }

    //Takes in Vector3 angularVelocity to rotate mobileCreature in local space
    protected void rotate(Vector3 angularVelocity)
    {
        if (shopMode) { Debug.LogWarning("Can't rotate in shop mode"); return; }
        //angularVelocity tells how many degrees to rotate in each axis
        Quaternion deltaRotation = Quaternion.Euler(angularVelocity * Time.fixedDeltaTime);
        mobileCreatureRB.MoveRotation(mobileCreatureRB.rotation * deltaRotation);
    }

    //Takes in Vector3 angularVelocity to rotate mobileCreature towards direction of vector
    protected void rotateTowards(Vector3 angularVelocity)
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
            setMaturity(percentage + maxEnergy / adultEnergy);
        }
    }

    /// <summary>
    /// set growth to this size, keeping size and energy proportional (unlike grow, which ADDS the percentage to the current size). Percentage cant be negative
    /// override creature setMaturity() function to add mobileCreature values that should scale
    /// </summary>
    public override void setMaturity(float percentage)
    {
        if (percentage <= 0) { Debug.LogWarning("maturity cannot be negative"); }
        setScaleTo(adultSize * percentage);
        maxEnergy = percentage * adultEnergy;
        huntingEnergyThreshold = .75f * maxEnergy;
        consumeRate = .5f * maxEnergy;
    }

    /// <summary> make identical copy (for now, in a random position nearby. This is probably temporary). </summary>
    public void duplicate<T>(Vector3 position, T partner) where T : MobileCreature
    {
        if (parentAquarium == null) { parentAquarium.setBreedingMutex(false); Debug.LogWarning("Could not find Aquarium parent"); return; }
        if (partner == null) { parentAquarium.setBreedingMutex(false);  return; } //Debug.LogWarning("Could not find breeding partner");
        if (!parentAquarium.isInBounds(position)) { parentAquarium.setBreedingMutex(false); return; } //keep w/in aquarium

        if (partner != null)
        {

        }
        print("Instantiating");

        MobileCreature child = (MobileCreature)parentAquarium.addEntity(childPrefab.GetComponent<Entity>(), position, transform.localRotation); //spawn nearby in same aquarium
        child.setChildValues(this, partner);
        count = 0;
        partner.count = 0;
        parentAquarium.setBreedingMutex(false);
        // print(partner.count);
        // print(partner.transform.position);
    }

    /// <summary> try to duplicate, but dont if there is a T too close to the attempted spawn location or too many of T in the aquarium as a whole. </summary>
    public override void tryDuplicate<T>(float minSpace = 0, float minCMCubedPerT = 0) //T constraint removed 
    {

        Vector3 randVecNearby = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius)) + transform.localPosition;

        MobileCreature closestT = parentAquarium.FindClosestOfType(this, randVecNearby);
        float closestTSqrDist = Mathf.Infinity;
        if (closestT != default(T)) { closestTSqrDist = getSqrDistBw(closestT.transform.localPosition, randVecNearby); }

        int numTInTank = parentAquarium.getAllOfType(this).Count;
        float currCMCubedPerT = Mathf.Infinity;
        if ((numTInTank > 0)) { currCMCubedPerT = parentAquarium.volume() / numTInTank; }

        MobileCreature potentialPartner = parentAquarium.FindClosestOfType(this);

        //Debug.Log("currPos "+ transform.localPosition);
        //Debug.Log("nrarbyPost "+ randVecNearby);
        //Debug.Log("closest "+ closestTSqrDist);
        //Debug.Log("curr units per T "+ currUnitsCubedPerT);
        if ((closestTSqrDist > minSpace * minSpace) && (minCMCubedPerT < currCMCubedPerT) && !(parentAquarium.getBreedingMutex()) && (potentialPartner))
        {
            print("Call duplicate");
            parentAquarium.setBreedingMutex(true);
            duplicate(randVecNearby, findPartner<MobileCreature>(potentialPartner));
        }
    }

    /// <summary> Find closest available creature of same type who is also ready to breed </summary>
    public T findPartner<T>(T closestT) where T : MobileCreature
    {
        if (closestT.readyToBreed() && this.GetType().Equals(closestT.GetType()))
        {
            return closestT;
        }
        return null;
    }

    /// <summary>
    /// if the creature is an adult, high energy, and past the breedingCooldown period
    /// </summary>
    public bool readyToBreed()
    {
        if (getScale() == adultSize)
        {
            if ((energy > adultEnergy * .75) && (count > breedingCooldown))
            {
                return true;
            }
        }
        return false;
    }

    public void predate(Creature creature)
    {
        

        predateCount += Time.deltaTime;
        if (predateCount < predateCooldown) return;
        else predateCount = 0;

        float damageDone = creature.beingEaten(consumeRate);
        if (creature.mustBeKilledToBeEaten && (damageDone >= consumeRate)) return;
        //did not kill the prey and prey must be killed to be eaten, so gets nothing. Have to check w a roundabout method because Destroy() does not work immediately
        //technically possible for prey to have the exact amount of energy as consumeRate and die and this creature gets no food anyways. lol
        eat(damageDone);
        if (damageDone > maxEnergy) grow(growthRate);
        else grow(growthRate * damageDone / maxEnergy); // it shouldnt grow fast from taking a bunch of tiny bites super fast
    }
    
    /// <summary>
    /// Calculates happiness of MobileCreature from energy level, friends, and decorations in tank
    /// </summary>
    /// <returns>float happiness value</returns>
    public override float getHappiness()
    {
        float happiness = energy / maxEnergy * 5;
        if (parentAquarium.getAllOfType(this).Count > 1) //if creature is not the last of its kind
        {
            happiness += 5;
        }

        Decoration[] decorations = parentAquarium.getAllOfType<Decoration>(); 
        happiness += decorations.Length * 5; //add based on qty of decorations

        foreach (Decoration d in decorations) //add based on individual decoration value
        {
            happiness += d.moneyBonus/2;
        }

        return happiness;
    }

}
