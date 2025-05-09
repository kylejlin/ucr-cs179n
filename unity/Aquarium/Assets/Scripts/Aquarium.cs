using UnityEngine;
using System.Collections.Generic;


public class Aquarium : MonoBehaviour
{
    private int id;
    public List<Entity> entities = new List<Entity>(); // all creatures (and objects?) within the tank
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 dimensions = new Vector3(50, 40, 50); // hard coded to fit basic aquarium, should be changable later (arbitrary water level of 40 cm)
    public float groundLevel = 6; //position of bottom plane of aquarium (temporary until we have a better ground)

    // Feel free to change this as needed.
    public float voxelSize = 5;

    public List<float> scentGradient = new List<float>();


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        updateScentGradient();
    }

    void updateScentGradient()
    {
        scentGradient.Clear();

        int xSize = (int)System.Math.Ceiling(dimensions.x / voxelSize);
        int ySize = (int)System.Math.Ceiling(dimensions.y / voxelSize);
        int zSize = (int)System.Math.Ceiling(dimensions.z / voxelSize);

        int numberOfVoxels = xSize * ySize * zSize;
        for (int i = 0; i < numberOfVoxels; ++i)
        {
            scentGradient.Add(0);
        }

        // This tracks the indices of the voxels that need to be filled.
        Queue<int> floodFillQueue = new Queue<int>();

        // For each food, set the voxel it occupies to `1` (which is the strongest scent),
        // and enqueue the voxel index.
        ImmobileCreature[] foods = (ImmobileCreature[])FindObjectsByType(typeof(ImmobileCreature), FindObjectsSortMode.None);
        for (int i = 0; i < numberOfVoxels; ++i)
        {
            int voxelX = i % xSize;
            int voxelY = (i / xSize) % ySize;
            int voxelZ = i / (xSize * ySize);

            // This assumes `transform.position` is at the center of the aquarium.
            Vector3 voxelPosition = transform.position - (dimensions / 2) + new Vector3(
                voxelX * voxelSize,
                voxelY * voxelSize,
                voxelZ * voxelSize
            );

            Vector3 voxelCenter = new Vector3(
                voxelPosition.x + (voxelSize / 2),
                voxelPosition.y + (voxelSize / 2),
                voxelPosition.z + (voxelSize / 2)
            );

            Bounds voxelBounds = new Bounds(voxelCenter, new Vector3(voxelSize, voxelSize, voxelSize));

            for (int j = 0; j < foods.Length; ++j)
            {
                Entity entity = foods[j];
                if (entity == null)
                {
                    continue;
                }

                // If the entity is within the voxel, set the scent to 1.
                if (voxelBounds.Contains(entity.transform.position))
                {
                    scentGradient[i] = 1;
                    floodFillQueue.Enqueue(i);

                    // No need to check other entities in this voxel,
                    // since we already know this voxel is occupied.
                    break;
                }
            }
        }

        // For each obstacle, set the voxel it occupies to `-1`, unless it's already equal to `1`.
        Entity[] obstacles = (Entity[])FindObjectsByType(typeof(Entity), FindObjectsSortMode.None);
        for (int i = 0; i < numberOfVoxels; ++i)
        {
            // If the voxel is already occupied by food, skip it.
            // Technically, every food is an obstacle, so we have to
            // check this special case or else the scent will be set to -1
            // (even for foods).
            if (scentGradient[i] == 1)
            {
                continue;
            }

            int voxelX = i % xSize;
            int voxelY = (i / xSize) % ySize;
            int voxelZ = i / (xSize * ySize);

            // This assumes `transform.position` is at the center of the aquarium.
            Vector3 voxelPosition = transform.position - (dimensions / 2) + new Vector3(
                voxelX * voxelSize,
                voxelY * voxelSize,
                voxelZ * voxelSize
            );

            Vector3 voxelCenter = new Vector3(
                voxelPosition.x + (voxelSize / 2),
                voxelPosition.y + (voxelSize / 2),
                voxelPosition.z + (voxelSize / 2)
            );

            Bounds voxelBounds = new Bounds(voxelCenter, new Vector3(voxelSize, voxelSize, voxelSize));

            for (int j = 0; j < obstacles.Length; ++j)
            {
                Entity entity = obstacles[j];
                if (entity == null)
                {
                    continue;
                }

                // If the entity is within the voxel, set the scent to -1.
                if (voxelBounds.Contains(entity.transform.position))
                {
                    scentGradient[i] = -1;
                    // No need to check other entities in this voxel,
                    // since we already know this voxel is occupied.
                    break;
                }
            }
        }

        // Flood fill the scent gradient to set the scent of each voxel.
        // This is O(n^3), so it could get slow for granular voxel tensors.

        while (floodFillQueue.Count > 0)
        {
            int voxelIndex = floodFillQueue.Dequeue();
            int voxelX = voxelIndex % xSize;
            int voxelY = (voxelIndex / xSize) % ySize;
            int voxelZ = voxelIndex / (xSize * ySize);

            // Check the 6 neighbors of the current voxel.
            for (int dx = -1; dx <= 1; dx += 2)
            {
                for (int dy = -1; dy <= 1; dy += 2)
                {
                    for (int dz = -1; dz <= 1; dz += 2)
                    {
                        if (dx != 0 && dy != 0 && dz != 0)
                        {
                            continue;
                        }

                        int neighborX = voxelX + dx;
                        int neighborY = voxelY + dy;
                        int neighborZ = voxelZ + dz;

                        if (neighborX < 0 || neighborX >= xSize ||
                            neighborY < 0 || neighborY >= ySize ||
                            neighborZ < 0 || neighborZ >= zSize)
                        {
                            continue;
                        }

                        int neighborIndex = neighborX + (neighborY * xSize) + (neighborZ * xSize * ySize);
                        if (scentGradient[neighborIndex] != -1 && scentGradient[neighborIndex] != 1)
                        {
                            // I'm going to use linear dropoff instead of quadratic,
                            // since we are using `max` instead of `add`.
                            scentGradient[neighborIndex] = System.Math.Max(scentGradient[neighborIndex], scentGradient[voxelIndex] - 0.01f);
                            floodFillQueue.Enqueue(neighborIndex);
                        }
                    }
                }
            }
        }

    }

    public void setID(int id)
    {
        this.id = id;
    }

    public Entity addEntity(Entity newEntity, Vector3 position, Quaternion rotation) //returns a reference to the newly created object 
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
        if (!local) localPos = transform.InverseTransformPoint(position);

        return ((localPos.x > -dimensions.x / 2) && (localPos.x < dimensions.x / 2)
            && (localPos.y >= groundLevel) && (localPos.y < dimensions.y)
            && (localPos.z > -dimensions.z / 2) && (localPos.z < dimensions.z / 2));
    }

    public float volume()
    {
        return dimensions.x * dimensions.y * dimensions.z;
    }

    public int calcCoin()
    {
        return 10; // todo: calculate the coins based on the number of creatures and decorations
    }

    public int getHunger()
    {
        return 20; // todo: calulate hunger percentage based on trilobites
    }
    public float getHappiness() // todo: calulate happiness bonus based on the decorations
    {
        return 0.10f;
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
