using UnityEngine;
using System.Collections.Generic; //list and dictionary definition


public class Aquarium : MonoBehaviour
{
    private int id;
    public List<Entity> entities = new List<Entity>(); // all creatures (and objects?) within the tank
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 dimensions = new Vector3(50, 40, 50); // hard coded to fit basic aquarium, should be changable later (arbitrary water level of 40 cm)
    public float groundLevel = 6; //position of bottom plane of aquarium (temporary until we have a better ground)
    private bool breedingMutex = false; //only allows one set of creatures to breed at a time

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        if(!local) localPos = transform.InverseTransformPoint(position);
        
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

    public bool getBreedingMutex()
    {
        return breedingMutex;
    }

    public void setBreedingMutex(bool value)
    {
        print("Set mutex");
        breedingMutex = value;
    }

}
