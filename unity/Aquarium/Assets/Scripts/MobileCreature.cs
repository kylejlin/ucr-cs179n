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

    // TODO: Restore this to `20` (or another more reasonable value) once
    // we finish debugging navigation.
    // For now, we set this outrageously high to force the creature
    // to always hunt.
    public static float huntingEnergyThreshold = 1000000;
    public static float metabolismRate = 1;
    public static float maxEatingDistance = 7;

    public BehaviorState state = BehaviorState.Idle;

    private Rigidbody mobileCreatureRB;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected new void Awake()
    {
        base.Awake(); //call Creature Start()
        mobileCreatureRB = GetComponent<Rigidbody>();
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


        // Try to eat if we're close enough.
        {
            Vector3 delta = closest.transform.position - transform.position;
            float distance = delta.magnitude;

            if (distance <= maxEatingDistance)
            {
                //Eat based on consumeRate 
                eat(closest.beingEaten(consumeRate));
                return;
            }
        }

        // Otherwise, perform gradient ascent.

        {
            // This assumes `transform.position` is at the center of the aquarium.
            Vector3 voxelOriginPosition = parentAquarium.transform.position - (parentAquarium.dimensions / 2);

            Vector3 positionRelativeToOrigin = transform.position - voxelOriginPosition;

            float voxelSize = parentAquarium.voxelSize;

            int xSize = parentAquarium.voxelGridXSize();
            int voxelXIndex = (int)(positionRelativeToOrigin.x / voxelSize);
            if (voxelXIndex < 0) { voxelXIndex = 0; }
            if (voxelXIndex >= xSize) { voxelXIndex = xSize - 1; }

            int ySize = parentAquarium.voxelGridYSize();
            int voxelYIndex = (int)(positionRelativeToOrigin.y / voxelSize);
            if (voxelYIndex < 0) { voxelYIndex = 0; }
            if (voxelYIndex >= ySize) { voxelYIndex = ySize - 1; }

            int zSize = parentAquarium.voxelGridZSize();
            int voxelZIndex = (int)(positionRelativeToOrigin.z / voxelSize);
            if (voxelZIndex < 0) { voxelZIndex = 0; }
            if (voxelZIndex >= zSize) { voxelZIndex = zSize - 1; }

            int bestVoxelXIndex = voxelXIndex;
            int bestVoxelYIndex = voxelYIndex;
            int bestVoxelZIndex = voxelZIndex;

            // Check neighboring voxel to find the one with the strongest scent.
            for (int dx = -1; dx <= 1; ++dx)
            {
                for (int dy = -1; dy <= 1; ++dy)
                {
                    for (int dz = -1; dz <= 1; ++dz)
                    {
                        int voxelXIndex2 = voxelXIndex + dx;
                        if (voxelXIndex2 < 0) { voxelXIndex2 = 0; }
                        if (voxelXIndex2 >= xSize) { voxelXIndex2 = xSize - 1; }

                        int voxelYIndex2 = voxelYIndex + dy;
                        if (voxelYIndex2 < 0) { voxelYIndex2 = 0; }
                        if (voxelYIndex2 >= ySize) { voxelYIndex2 = ySize - 1; }

                        int voxelZIndex2 = voxelZIndex + dz;
                        if (voxelZIndex2 < 0) { voxelZIndex2 = 0; }
                        if (voxelZIndex2 >= zSize) { voxelZIndex2 = zSize - 1; }


                        float scent = parentAquarium.scentAt(voxelXIndex2, voxelYIndex2, voxelZIndex2);
                        if (scent > parentAquarium.scentAt(bestVoxelXIndex, bestVoxelYIndex, bestVoxelZIndex))
                        {
                            bestVoxelXIndex = voxelXIndex2;
                            bestVoxelYIndex = voxelYIndex2;
                            bestVoxelZIndex = voxelZIndex2;
                        }
                    }
                }
            }

            Vector3 destination = new Vector3(
                bestVoxelXIndex * voxelSize,
                bestVoxelYIndex * voxelSize,
                bestVoxelZIndex * voxelSize
            ) + voxelOriginPosition;

            Vector3 delta = destination - transform.position;

            Vector3 displacementNormal = delta.normalized;
            float k = speed * Time.deltaTime;
            displacementNormal.Scale(new Vector3(k, k, k));
            transform.position += displacementNormal;
            rotateTowards(delta);
        }
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

}
