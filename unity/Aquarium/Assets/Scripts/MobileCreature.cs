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
    public int consumeRate = 10;
    public float speed = 1;

    public static float huntingEnergyThreshold = 20;
    public static float metabolismRate = 1;
    public static float maxEatingDistance = 7;

    public BehaviorState state = BehaviorState.Idle;

    private Rigidbody mobileCreatureRB;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected new void Awake()
    {
        base.Awake(); //call Creature Start()
        mobileCreatureRB = GetComponent<Rigidbody>();

        name = "Trilobite "+ entityName;
        growthRate = 0.1f; 
        adultEnergy = 40; 
        energy = 40;
        maxEnergy = 40;

        spawnSize = 1f; //for demo, have them spawn fully grown
        spawnRadius = 5;
        minSpawnSpace = 1; 
        minCMCubedPer = 2000;
        initSize();
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
            speed = 1;
            UpdateIdle();
        }
        else if (state == BehaviorState.Hunting)
        {
            speed = 3;
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
        if(transform.localRotation.eulerAngles.x > 45){ //if creature pointed downwards, rotate upwards
            // Vector3 angVec = new Vector3(30, 0, 0); 
            // rotate(angVec);
        }else{
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
        if(shopMode) {Debug.LogWarning("Can't move in shop mode"); return; }
        //using rigidbody.MovePosition() will make transitioning to the new position smoother if interpolation is enabled
        //MovePosition(currentPosition + displacement)
        mobileCreatureRB.MovePosition(mobileCreatureRB.position + mobileCreatureRB.rotation * velocity * speed * Time.fixedDeltaTime);
    }

    //Takes in Vector3 angularVelocity to rotate mobileCreature
    private void rotate(Vector3 angularVelocity)
    {
        if(shopMode) {Debug.LogWarning("Can't rotate in shop mode"); return; }
        //angularVelocity tells how many degrees to rotate in each axis
        Quaternion deltaRotation = Quaternion.Euler(angularVelocity * Time.fixedDeltaTime);
        mobileCreatureRB.MoveRotation(mobileCreatureRB.rotation * deltaRotation);
    }

    //Takes in Vector3 angularVelocity to rotate mobileCreature towards direction of vector
    private void rotateTowards(Vector3 angularVelocity)
    {   
        if(shopMode) {Debug.LogWarning("Can't rotate in shop mode"); return; }
        //rotates creature with respect to front of creature (head points towards rotation)
        mobileCreatureRB.MoveRotation(Quaternion.LookRotation(angularVelocity,Vector3.forward));
    }



}
