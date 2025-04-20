using UnityEngine;

public enum BehaviorState
{
    Idle,
    Hunting,
    Dying,
}

public class MobileCreature : Creature
{   
    
    public bool canSwim = false;
    public int consumeRate;
    public float speed = 1;

    public static float maxEnergy = 100;

    public float energy = maxEnergy;

    public static float huntingEnergyThreshold = 50;


    public static float metabolismRate = 1;

    public BehaviorState state = BehaviorState.Idle;

    public static float maxEatingDistance = 5;

    private Rigidbody mobileCreatureRB;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mobileCreatureRB = GetComponent<Rigidbody>();
        MobileCreature friend = FindClosest<MobileCreature>();
        if (friend == default(MobileCreature)) Debug.Log("None found");
        else Debug.Log("nearest trilo pos:" + friend.transform.localPosition.x);
    }

    // FixedUpdate is called at fixed time intervals
    void FixedUpdate()
    {   
        energy -= Time.deltaTime * metabolismRate;

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
        Vector3 vec = new Vector3(0,0,1); //move forward
        move(vec);
        Vector3 angVec = new Vector3(0, 10, 0); //rotate a little bit around axes
        rotate(angVec);
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
            // For now, I implemented eating as instantaneous.
            // We can always adjust this later if you want a more gradual shrinking.
            energy = maxEnergy;
            closest.beingEaten(closest.maxHealth);
            return;
        }

        Vector3 displacement = delta.normalized;
        float k = speed * Time.deltaTime;
        displacement.Scale(new Vector3(k, k, k));
        transform.position += displacement;
        rotateTowards(delta);
    }

    //Takes in Vector3 velocity to move mobileCreature
    private void move(Vector3 velocity)
    {   
        //using rigidbody.MovePosition() will make transitioning to the new position smoother if interpolation is enabled
        //MovePosition(currentPosition + displacement)
        mobileCreatureRB.MovePosition(mobileCreatureRB.position + mobileCreatureRB.rotation * velocity * speed * Time.fixedDeltaTime);
    }

    //Takes in Vector3 angularVelocity to rotate mobileCreature
    private void rotate(Vector3 angularVelocity)
    {
        //angularVelocity tells how many degrees to rotate in each axis
        Quaternion deltaRotation = Quaternion.Euler(angularVelocity * Time.fixedDeltaTime);
        mobileCreatureRB.MoveRotation(mobileCreatureRB.rotation * deltaRotation);
    }

    //Takes in Vector3 angularVelocity to rotate mobileCreature towards direction of vector
    private void rotateTowards(Vector3 angularVelocity)
    {   
        //rotates creature with respect to front of creature (head points towards rotation)
        mobileCreatureRB.MoveRotation(Quaternion.LookRotation(angularVelocity,Vector3.forward));
    }
}
