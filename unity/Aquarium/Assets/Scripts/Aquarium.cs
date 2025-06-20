using UnityEngine;
using System.Collections.Generic; //list and dictionary definition


public class Aquarium : MonoBehaviour
{
    private int id;
    public List<Entity> entities = new List<Entity>(); // all creatures (and objects?) within the tank
    public Vector3 dimensions; // hard coded to fit basic aquarium, should be changable later, set in inspector
    public float groundLevel; //position of bottom plane of aquarium, set in inspector
    private bool breedingMutex = false; //only allows one set of creatures to breed at a time

    public List<float> voxelGridBuf = new List<float>();
    public float voxelSize = 2;
    public bool sphereDebug;
    public GameObject debugSphere; //sphere to copy
    public List<GameObject> spheres = new List<GameObject>();
    // public Vector3Int testVoxel = new Vector3Int(10, 10, 10);


    void Start()
    {
        if (sphereDebug)
        {
            spheres.Clear();
            for (int i = 0; i < requiredVoxelGridBufSize(); ++i)
            {
                spheres.Add(Instantiate(debugSphere, voxelCoordsToAquariumCoords(bufIndexToVoxelCoords(i)), Quaternion.identity, transform));
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        UpdateScentGradient();
        // PrintScentGradient();
    }

    void PrintScentGradient()
    {
        Vector3Int gridSize = voxelGridSize();
        string s = "";
        {
            int x = gridSize.x;
            int y = gridSize.y;
            int z = gridSize.z;
            s += $"GRID({x}x{y}x{z}):\n\n";
        }

        for (int z = 0; z < gridSize.z; ++z)
        {
            s += "\n\n\n\nz=" + z + "[\n";
            for (int y = 0; y < gridSize.y; ++y)
            {
                for (int x = 0; x < gridSize.x; ++x)
                {
                    s += formatScentLevel(getScentAt(new Vector3Int(x, y, z))) + " ";
                }
                s += "\n";
            }
            s += "]\n";
        }

        Debug.Log(s);
    }

    static string formatScentLevel(float scentLevel)
    {
        if (scentLevel < 0f)
        {
            return "x";
        }

        // int v = (int)System.Math.Floor(scentLevel * 100) - 100;
        // if (v >= -25)
        // {
        //     char c = (char)('Z' + v);
        //     return c.ToString();
        // }
        // return "?";

        return scentLevel.ToString();
    }

    public void setID(int id)
    {
        this.id = id;
    }

    public Entity addEntity(Entity newEntity, Vector3 position, Quaternion rotation) //returns a reference to the newly created object //GLOBAL positions
    {
        if (newEntity == null) { Debug.LogWarning("Null entity passed into addEntity"); return null; }
        if (!isInBounds(position)) { Debug.Log("not within bounds " + position); return null; } //keep within aquarium bounds

        Entity e = Instantiate(newEntity.gameObject, position, rotation, gameObject.transform).GetComponent<Entity>();
        entities.Add(e);
        e.parentAquarium = this;


        return e;
    }

    public Entity addEntity(Entity newEntity, bool onGround = true) //temporary. we need a system so the shop knows what to spawn on the ground or not. (bool in entity?)
    {
        if (newEntity == null) { Debug.LogWarning("Null entity passed into addEntity"); return null; }
        Vector3 randomPosition = new Vector3(
            Random.Range(-dimensions.x / 2, dimensions.x / 2),
            Random.Range(groundLevel, dimensions.y),
            Random.Range(-dimensions.z / 2, dimensions.z / 2)
        );

        if (onGround) randomPosition.y = groundLevel;

        // want all creatures to be upright I think?
        // Quaternion randomRotation = Quaternion.Euler(
        //     Random.Range(0, 360),
        //     Random.Range(0, 360),
        //     Random.Range(0, 360)
        // );

        return addEntity(newEntity, randomPosition, Quaternion.identity);
    }

    public Entity removeEntity(Entity entity)
    {
        if (entity == null) { Debug.LogWarning("Null entity passed into removeEntity"); return null; }
        if (entities.Remove(entity)) { return entity; }
        else { Debug.LogWarning("Entity not found in entity list"); return null; }

    }

    public bool isInBounds(Vector3 position, bool local = false)
    {
        Vector3 localPos = position;
        float accuracy = 0.0001f;
        if (!local) localPos = transform.InverseTransformPoint(position);

        return ((localPos.x >= -dimensions.x / 2 - accuracy) && (localPos.x <= dimensions.x / 2 + accuracy)
            && (localPos.y >= groundLevel - accuracy) && (localPos.y <= dimensions.y + accuracy)
            && (localPos.z >= -dimensions.z / 2 - accuracy) && (localPos.z <= dimensions.z / 2 + accuracy));
    }

    public float volume()
    {
        return dimensions.x * dimensions.y * dimensions.z;
    }


    public bool getBreedingMutex()
    {
        return breedingMutex;
    }

    public void setBreedingMutex(bool value)
    {
        // print("Set mutex");
        breedingMutex = value;
    }

    ///<summary> Must specify the class to search for in the <>. Finds closest one to aquariumPosition (in the same tank only). </summary>///
    /// <returns> entity of type T that is closest to Position (in aquarium space) or null if there are none found. </returns>
    public T FindClosest<T>(Vector3 aquariumPosition) where T : Entity  //get all objects of one type, then check their positions and return the closest (excluding self)
    {
        T[] foundEntities = getAllOfType<T>();
        if (foundEntities == null) return default(T);

        T closest = default(T);
        foreach (T e in foundEntities)
        {
            if (!e.enabled) continue;

            float newDist = getSqrDistBw(aquariumPosition, transform.InverseTransformVector(e.transform.position));

            if (closest == default(T)) closest = e;
            else if ((newDist < getSqrDistBw(aquariumPosition, closest.transform.localPosition))) closest = e;
        }
        return closest;
    }

    ///<summary> Must specify the class to search for in the <>. Finds closest one to the entity given (does NOT need to be the same type). </summary>///
    /// <returns> entity of type T that is closest to Position (in aquarium space) or null if there are none found. if excludeSelf it will exclude itself by checking its uniqueID </returns>
    public T FindClosest<T>(Entity entity, bool excludeSelf = true) where T : Entity  //get all objects of one type, then check their positions and return the closest (excluding self)
    {
        T[] foundEntities = getAllOfType<T>();
        if (foundEntities == null) return default(T);

        T closest = default(T);
        foreach (T e in foundEntities)
        {
            if (!e.enabled) continue;

            float newDist = getSqrDistBwEntities(entity, e);
            if (excludeSelf && (e.getUniqueID() == entity.getUniqueID())) continue; // dont count yourself

            if (closest == default(T)) closest = e;
            else if ((newDist < getSqrDistBwEntities(entity, closest))) closest = e;
        }
        return closest;
    }
    /// <summary> returns an array of all of the entities in the tank with the type T. Can be any type in the inheritance hierarchy. If a parent class calls this it will search for that parent class type </summary>
    /// <returns> array of type T of all the found entities in this aquarium, or null if none were found </returns>
    public T[] getAllOfType<T>() where T : Entity
    {
        return GetComponentsInChildren<T>();
    }


    ///<summary> Same as above, but it will search for ONLY the most derived class of entity, even if called in a parent class. 
    /// Since entity is of type T, you do NOT need to specify the type on the <>. You can call like: parentAquarium.FindClosestOfType(this, true) </summary>
    /// <returns> entity of type of T passed in that is closest to Position (in aquarium space) or null if there are none found. </returns>
    public T FindClosestOfType<T>(T entity, Vector3 aquariumPosition) where T : Entity  //get all objects of one type, then check their positions and return the closest (excluding self)
    {
        List<Entity> foundEntities = getAllOfType(entity);
        if (foundEntities == null) return default(T);

        T closest = default(T);
        foreach (T e in foundEntities)
        {
            if (!e.enabled) continue;

            float newDist = getSqrDistBw(aquariumPosition, transform.InverseTransformVector(e.transform.position));

            if (closest == default(T)) closest = e;
            else if ((newDist < getSqrDistBw(aquariumPosition, closest.transform.localPosition))) closest = e;
        }
        return closest;
    }
    

    ///<summary> Same as above, but it will search for ONLY the most derived class of entity, even if called in a parent class. 
    /// Since entity is of type T, you do NOT need to specify the type on the <>. You can call like: parentAquarium.FindClosestOfType(this, true) </summary>
    /// <returns> entity of type of T passed in that is closest to the position of entity, excluding entity itself if excludeSelf, or null if there are none found. </returns>
    public T FindClosestOfType<T>(T entity, bool excludeSelf = true) where T : Entity //get all objects of one type, then check their positions and return the closest (excluding self)
    {
        List<Entity> foundEntities = getAllOfType(entity);
        if (foundEntities == null) return null;

        T closest = default(T);
        foreach (T e in foundEntities)
        {
            if (!e.enabled) continue;

            float newDist = getSqrDistBwEntities(entity, e);
            if (excludeSelf && (e.getUniqueID() == entity.getUniqueID())) continue; // dont count yourself

            if (closest == default(T)) closest = e;
            else if (newDist < getSqrDistBwEntities(entity, closest)) closest = e;
        }
        return closest;
    }

    /// <summary> Return list of all entities in the tank with the type of the entity passed in. Uses the MOST derived class of entity, even if called by a parent class. </summary>
    public List<Entity> getAllOfType(Entity entity)
    {
        List<Entity> allEntities = entities.FindAll(x => x.GetType() == entity.GetType());
        if (allEntities == null)
        {
            return null;
        }
        return allEntities; //predicate defining a condition of what to find 
    }


    /// <summary> gets the bounding boxes of all entities of type T in the aquarium, with their coords in the local aquarium space</summary>
    public List<Bounds> getBoundsInAquariumCoords<T>() where T : Entity
    {
        T[] foundEntities = getAllOfType<T>();
        List<Bounds> aquariumBoundsList = new List<Bounds>();
        Vector3 newCenter = new Vector3();
        foreach (T e in foundEntities)
        { //get bounds in world space
            Collider[] colliders = e.GetComponentsInChildren<Collider>(); //get references to all colliders in that entity
            foreach (Collider c in colliders)
            {
                newCenter = e.transform.TransformVector(c.bounds.center - e.transform.position) + e.transform.position - this.transform.position; //for some reason the Bounds are in the correct worldspce position, but are scaled locally. So you have to do this mess
                //this is a mess. theres gotta be a better way
                aquariumBoundsList.Add(new Bounds(newCenter, e.transform.TransformVector(c.bounds.size) / transform.localScale.x));
                //this assumes that the aquarium is a root and that it has proportional scale and isnt rotated
            }
        }
        return aquariumBoundsList;

    }

    /// <summary> returns the objects position relative to this aquarium, regardless of its position in the hierarchy</summary>
    public Vector3 getAquariumSpacePosition(GameObject obj)
    {
        return transform.InverseTransformVector(obj.transform.position);
    }
    public float getSqrDistBw(Vector3 vec1, Vector3 vec2) { return (vec1 - vec2).sqrMagnitude; }
    public float getSqrDistBwEntities(Entity e1, Entity e2) { return getSqrDistBw(e1.transform.position, e2.transform.position); }
    public Vector3 getMinAquariumCoords() { return new Vector3(-dimensions.x / 2, groundLevel, -dimensions.z / 2); } //hard coded for aquarium default value w scale 1
    public Vector3 getMaxAquariumCoords() { return new Vector3(dimensions.x / 2, dimensions.y, dimensions.z / 2); } //hard coded for aquarium default value w scale 1
    public Vector3 transformAquariumCoordsToWorldCoords(Vector3 aquariumCoords) { return aquariumCoords + transform.position; } //assuming theres no scale or rotation


    public void UpdateScentGradient()
    {
        voxelGridBuf.Clear();

        int requiredBufSize = requiredVoxelGridBufSize();
        for (int i = 0; i < requiredBufSize; ++i)
        {
            voxelGridBuf.Add(0f);
        }

        // Forgive the bad grammar ("boundses").
        // The problem is that "bounds" is already plural,
        // so we need a "double plural" to describe a list of bounds.

        // For now, we assume the only scent blockers are creatures.
        // This means the navigation algorithm will ignore non-creatures (e.g., decorations).
        // However, we can expand this later.
        List<Bounds> scentBlockerBoundses = getBoundsInAquariumCoords<Decoration>();

        // For now, we assume the only food sources are immobile creatures.
        List<Bounds> foodBoundses = getBoundsInAquariumCoords<ImmobileCreature>();

        List<Vector3Int> deltasToNeighborsExcludingSelf = getDeltasToNeighborsExcludingSelf();

        Queue<int> floodBufIndices = new Queue<int>();

        for (int i = 0; i < requiredBufSize; ++i)
        {
            Bounds voxelBounds = bufIndexToVoxelBounds(i);

            foreach (Bounds blockerBounds in scentBlockerBoundses)
            {
                if (blockerBounds.Intersects(voxelBounds))
                {
                    voxelGridBuf[i] = -1f;
                    break;
                }
            }

            foreach (Bounds foodBounds in foodBoundses)
            {
                if (foodBounds.Intersects(voxelBounds))
                {
                    voxelGridBuf[i] = 1f;
                    floodBufIndices.Enqueue(i);
                    break;
                }
            }
        }

        while (floodBufIndices.Count > 0)
        {
            if (floodBufIndices.Count > requiredBufSize)
            {
                Debug.LogWarning("Flood buffer indices count exceeded required buffer size. FAILING.");
                break;
            }

            int bufIndex = floodBufIndices.Dequeue();

            float newNeighborScentValue = voxelGridBuf[bufIndex] - 0.01f;

            if (newNeighborScentValue <= 0f)
            {
                // The scent is too weak to be useful,
                // so we skip this queue item.
                continue;
            }

            Vector3Int voxelCoords = bufIndexToVoxelCoords(bufIndex);

            foreach (Vector3Int delta in deltasToNeighborsExcludingSelf)
            {
                Vector3Int neighborVoxelCoords = voxelCoords + delta;
                if (!areVoxelCoordsValid(neighborVoxelCoords))
                {
                    continue;
                }



                float oldNeighborScentValue = getScentAt(neighborVoxelCoords);

                if (oldNeighborScentValue < 0f)
                {
                    // This voxel is blocked; we cannot flood it.
                    continue;
                }

                if (newNeighborScentValue <= oldNeighborScentValue)
                {
                    // This voxel was already flooded with a stronger scent.
                    // We must not flood it again, or else we will create an infinite loop.
                    continue;
                }

                int neighborBufIndex = voxelCoordsToBufIndex(neighborVoxelCoords);
                voxelGridBuf[neighborBufIndex] = newNeighborScentValue;
                floodBufIndices.Enqueue(neighborBufIndex);

                // Debug.Log($"Enqueued voxel at {neighborVoxelCoords} with scent value {newNeighborScentValue}");
            }
        }
        // debugSphere.transform.localPosition = voxelCoordsToAquariumCoords(testVoxel);
        // float scentStrenght = voxelGridBuf[voxelCoordsToBufIndex(testVoxel)];
        // debugSphere.transform.localScale = new Vector3(scentStrenght, scentStrenght, scentStrenght);
        if (sphereDebug)
        {
            List<int> targetedBufIndices = new List<int>();

            {
                MobileCreature[] foundEntities = getAllOfType<MobileCreature>();
                foreach (MobileCreature e in foundEntities)
                {
                    Vector3Int targetVoxelCoords = e.getTargetPositionInVoxelCoords();
                    if (!areVoxelCoordsValid(targetVoxelCoords))
                    {
                        continue;
                    }

                    int targetBufIndex = voxelCoordsToBufIndex(targetVoxelCoords);

                    if (targetedBufIndices.Contains(targetBufIndex))
                    {
                        continue; // The index is already in the list, so we skip it.
                    }

                    targetedBufIndices.Add(targetBufIndex);
                }
            }

            for (int i = 0; i < voxelGridBuf.Count; i++)
            {
                if (voxelGridBuf[i] >= 0) spheres[i].transform.localScale = new Vector3(voxelGridBuf[i], voxelGridBuf[i], voxelGridBuf[i]);
                else spheres[i].transform.localScale = Vector3.zero;

                // if (targetedBufIndices.Contains(i))
                // {
                //     float big = 10f;
                //     spheres[i].transform.localScale = new Vector3(big, big, big);
                // }
            }
        }
    }

    public Vector3Int voxelGridSize()
    {
        Vector3 gridDimensions = getMaxAquariumCoords() - getMinAquariumCoords();
        Vector3Int gridSize = new Vector3Int();
        gridSize.x = Mathf.CeilToInt(gridDimensions.x / voxelSize);
        gridSize.y = Mathf.CeilToInt(dimensions.y / voxelSize);
        gridSize.z = Mathf.CeilToInt(dimensions.z / voxelSize);
        return gridSize;
    }

    public bool areVoxelCoordsValid(Vector3Int voxelCoords)
    {
        Vector3Int gridSize = voxelGridSize();
        return (voxelCoords.x >= 0 && voxelCoords.x < gridSize.x
            && voxelCoords.y >= 0 && voxelCoords.y < gridSize.y
            && voxelCoords.z >= 0 && voxelCoords.z < gridSize.z);
    }

    public int requiredVoxelGridBufSize()
    {
        Vector3Int gridSize = voxelGridSize();
        return gridSize.x * gridSize.y * gridSize.z;
    }

    public Vector3Int bufIndexToVoxelCoords(int index)
    {
        Vector3Int gridSize = voxelGridSize();
        int x = index % gridSize.x;
        int y = index / gridSize.x % gridSize.y;
        int z = index / (gridSize.x * gridSize.y);
        return new Vector3Int(x, y, z);
    }

    public int voxelCoordsToBufIndex(Vector3Int voxelCoords)
    {
        Vector3Int gridSize = voxelGridSize();
        return voxelCoords.x + (voxelCoords.y * gridSize.x) + (voxelCoords.z * gridSize.x * gridSize.y);
    }

    public float getScentAt(Vector3Int voxelCoords)
    {
        int bufIndex = voxelCoordsToBufIndex(voxelCoords);
        if ((bufIndex < 0) || (bufIndex >= voxelGridBuf.Count))
        {
            Debug.LogWarning("bufIndex out of bounds");
            return 0f;
        }
        return voxelGridBuf[bufIndex];
    }

    public Bounds bufIndexToVoxelBounds(int index)
    {
        Vector3Int voxelCoords = bufIndexToVoxelCoords(index);
        Vector3 min = getMinAquariumCoords() + new Vector3(voxelCoords.x * voxelSize, voxelCoords.y * voxelSize, voxelCoords.z * voxelSize);
        Vector3 max = min + new Vector3(voxelSize, voxelSize, voxelSize);
        return new Bounds((min + max) / 2f, max - min);
    }
    public Vector3 voxelCoordsToAquariumCoords(Vector3 voxelCoords)
    {
        Vector3 min = getMinAquariumCoords() + new Vector3(voxelCoords.x * voxelSize, voxelCoords.y * voxelSize, voxelCoords.z * voxelSize);
        Vector3 max = min + new Vector3(voxelSize, voxelSize, voxelSize);
        return (max + min) * 0.5f; //average of max and min is the center of the voxel in aquarium coords
    }

    public static List<Vector3Int> getDeltasToNeighborsExcludingSelf()
    {
        List<Vector3Int> deltas = new List<Vector3Int>();

        for (int dx = -1; dx <= 1; ++dx)
        {
            for (int dy = -1; dy <= 1; ++dy)
            {
                for (int dz = -1; dz <= 1; ++dz)
                {
                    if (dx == 0 && dy == 0 && dz == 0)
                    {
                        continue;
                    }
                    deltas.Add(new Vector3Int(dx, dy, dz));
                }
            }
        }

        return deltas;
    }

    public Vector3Int getBestNeighborCoordsInVoxelCoords(Vector3 startInAquariumCoords)
    {
        // Vector3 minAquariumCoords = getMinAquariumCoords();
        // Vector3 maxAquariumCoords = getMaxAquariumCoords();

        // if (!(
        //     minAquariumCoords.x <= startInAquariumCoords.x && startInAquariumCoords.x <= maxAquariumCoords.x
        //     && minAquariumCoords.y <= startInAquariumCoords.y && startInAquariumCoords.y <= maxAquariumCoords.y
        //     && minAquariumCoords.z <= startInAquariumCoords.z && startInAquariumCoords.z <= maxAquariumCoords.z))
        // {
        //     {
        //         Debug.LogWarning($"startInAquariumCoords is not in aquarium bounds. startInAquariumCoords: {startInAquariumCoords}; minAquariumCoords: {minAquariumCoords}; maxAquariumCoords: {maxAquariumCoords}");
        //         return new Vector3Int(-1, -1, -1);
        //     }
        // }

        // Vector3 startRelativeToMin = startInAquariumCoords - minAquariumCoords;
        // Vector3Int startVoxelCoords = new Vector3Int(
        //     Mathf.FloorToInt(startRelativeToMin.x / voxelSize),
        //     Mathf.FloorToInt(startRelativeToMin.y / voxelSize),
        //     Mathf.FloorToInt(startRelativeToMin.z / voxelSize)
        // );
        // if (!areVoxelCoordsValid(startVoxelCoords))
        // {
        //     Debug.LogWarning("startVoxelCoords is not valid");
        //     return new Vector3Int(-1, -1, -1);
        // }

        Vector3Int startVoxelCoords = aquariumCoordsToVoxelCoords(startInAquariumCoords);
        Vector3Int bestVoxelCoords = startVoxelCoords;
        

        List<Vector3Int> deltasToNeighborsExcludingSelf = getDeltasToNeighborsExcludingSelf();
        foreach (Vector3Int delta in deltasToNeighborsExcludingSelf)
        {
            Vector3Int neighborVoxelCoords = startVoxelCoords + delta;
            if (!areVoxelCoordsValid(neighborVoxelCoords))
            {
                continue;
            }

            if (getScentAt(neighborVoxelCoords) > getScentAt(bestVoxelCoords))
            {
                bestVoxelCoords = neighborVoxelCoords;
            }
        }

        // spheres[voxelCoordsToBufIndex(bestVoxelCoords)].transform.localScale = new Vector3(10, 10, 10);
        // print(bestVoxelCoords);
        // print("world coords: "+voxelCoordsToAquariumCoords(bestVoxelCoords));
        return bestVoxelCoords;
    }

    public Vector3 getBestNeighborCoordsInAquariumCoords(Vector3 startInAquariumCoords)
    {
        Vector3Int bestVoxelCoords = getBestNeighborCoordsInVoxelCoords(startInAquariumCoords);
        return voxelCoordsToAquariumCoords(bestVoxelCoords);
    }

    public Vector3Int aquariumCoordsToVoxelCoords(Vector3 startInAquariumCoords)
    {
        Vector3 minAquariumCoords = getMinAquariumCoords();
        Vector3 maxAquariumCoords = getMaxAquariumCoords();

        if (!(
            minAquariumCoords.x <= startInAquariumCoords.x && startInAquariumCoords.x <= maxAquariumCoords.x
            && minAquariumCoords.y <= startInAquariumCoords.y && startInAquariumCoords.y <= maxAquariumCoords.y
            && minAquariumCoords.z <= startInAquariumCoords.z && startInAquariumCoords.z <= maxAquariumCoords.z))
        {
            {
                Debug.LogWarning($"startInAquariumCoords is not in aquarium bounds. startInAquariumCoords: {startInAquariumCoords}; minAquariumCoords: {minAquariumCoords}; maxAquariumCoords: {maxAquariumCoords}");
                return new Vector3Int(-1, -1, -1);
            }
        }

        Vector3 startRelativeToMin = startInAquariumCoords - minAquariumCoords;
        Vector3Int startVoxelCoords = new Vector3Int(
            Mathf.FloorToInt(startRelativeToMin.x / voxelSize),
            Mathf.FloorToInt(startRelativeToMin.y / voxelSize),
            Mathf.FloorToInt(startRelativeToMin.z / voxelSize)
        );
        if (!areVoxelCoordsValid(startVoxelCoords))
        {
            Debug.LogWarning("startVoxelCoords is not valid");
            return new Vector3Int(-1, -1, -1);
        }
        return startVoxelCoords;
    }

    //check voxel in path of creatures movement to check for obstacles to avoid. todo: check whole ray of voxels? it will fail for a thin barrier
    public bool checkVoxelInFrontForObstacle(Vector3 startInAquariumCoords, Vector3 localFacingDirection, float lookAheadDist = 2)
    {
        Vector3 positionToCheck = startInAquariumCoords + (localFacingDirection.normalized * lookAheadDist);
        if (!isInBounds(positionToCheck)) return false;
        Vector3Int bestVoxelCoords = aquariumCoordsToVoxelCoords(positionToCheck);
        // Vector3Int voxelInFront = bestVoxelCoords;
        // if ((localFacingDirection.x * localFacingDirection.x >= localFacingDirection.y * localFacingDirection.y) && (localFacingDirection.x * localFacingDirection.x >= localFacingDirection.z * localFacingDirection.z))
        // {
        //     if (localFacingDirection.x < 0) voxelInFront.x -= lookAheadVoxels;
        //     else voxelInFront.x += lookAheadVoxels;
        // }
        // else if ((localFacingDirection.y * localFacingDirection.y >= localFacingDirection.x * localFacingDirection.x) && (localFacingDirection.y * localFacingDirection.y >= localFacingDirection.z * localFacingDirection.z))
        // {
        //     if (localFacingDirection.y < 0) voxelInFront.y -= lookAheadVoxels;
        //     else voxelInFront.y += lookAheadVoxels;
        // }
        // else
        // {
        //     if (localFacingDirection.z < 0) voxelInFront.z -= lookAheadVoxels;
        //     else voxelInFront.z += lookAheadVoxels;
        // }


        if (getScentAt(bestVoxelCoords) < 0) return false;
        if (!areVoxelCoordsValid(bestVoxelCoords)) return false;
        return true;
    }



    public float calcMoney()
    {
        float currMoneyBonus = 1f; //start w 1 for the tank itself
        foreach (Entity e in entities) currMoneyBonus += e.calcMoneyBonus();
        return currMoneyBonus;

    }

    public int getHunger()
    {
        return 20; // todo: calulate hunger percentage based on trilobites
    }
    public float getHappiness() 
    {
        float currHappiness = 1;
        foreach (Entity e in entities) currHappiness += e.getHappiness();
        return currHappiness;
    }
    public float getMaxHappiness()
    {
        float maxHappiness = 1;
        foreach (Entity e in entities) maxHappiness += e.maxHappiness;
        return maxHappiness;
    }
    public int getAlgaesHealth() // todo: calulate algaes health based on algaes
    {
        return 10;
    }
    public int getTrilobitesConsume() // todo: calulate total consume based on algaes
    {
        return 10;
    }

}
