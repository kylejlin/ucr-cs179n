using UnityEngine;
using System.Collections.Generic; //list and dictionary definition


public class Aquarium : MonoBehaviour
{
    private int id;
    public List<Entity> entities = new List<Entity>(); // all creatures (and objects?) within the tank
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 dimensions; // hard coded to fit basic aquarium, should be changable later, set in inspector
    public float groundLevel; //position of bottom plane of aquarium, set in inspector
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
        float accuracy = 0.0001f;
        if(!local) localPos = transform.InverseTransformPoint(position);

        // if (!(localPos.x >= -dimensions.x / 2 - accuracy) && (localPos.x <= dimensions.x / 2 + accuracy)) print("xpos problem");
        // if (!(localPos.y >= groundLevel - accuracy)) print("ypos too low");
        // if (!(localPos.y <= dimensions.y + accuracy)) print("ypos too high");
        // if (!(localPos.z >= -dimensions.z / 2 - accuracy) && (localPos.z <= dimensions.z / 2 + accuracy)) print("zpos problem");
        // if (!((localPos.x >= -dimensions.x / 2 - accuracy) && (localPos.x <= dimensions.x / 2 + accuracy)
        //     && (localPos.y >= groundLevel - accuracy) && (localPos.y <= dimensions.y + accuracy)
        //     && (localPos.z >= -dimensions.z / 2 - accuracy) && (localPos.z <= dimensions.z / 2 + accuracy))) print("returning fasle");

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
        print("Set mutex");
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
        foreach(T e in foundEntities){ //get bounds in world space
            Collider[] colliders = e.GetComponentsInChildren<Collider>(); //get references to all colliders in that entity
            foreach(Collider c in colliders){
                newCenter = e.transform.TransformVector(c.bounds.center - e.transform.position) + e.transform.position - this.transform.position; //for some reason the Bounds are in the correct worldspce position, but are scaled locally. So you have to do this mess
                //this is a mess. theres gotta be a better way
                aquariumBoundsList.Add(new Bounds(newCenter, e.transform.TransformVector(c.bounds.size) / transform.localScale.x)); 
                //this assumes that the aquarium is a root and that it has proportional scale and isnt rotated
            }
        }
        return aquariumBoundsList;
        
    }

    /// <summary> returns the objects position relative to this aquarium, regardless of its position in the hierarchy</summary>
    public Vector3 getAquariumSpacePosition(GameObject obj){
        return transform.InverseTransformVector(obj.transform.position);
    }
    public float getSqrDistBw(Vector3 vec1, Vector3 vec2) { return (vec1 - vec2).sqrMagnitude; }
    public float getSqrDistBwEntities(Entity e1, Entity e2) {return getSqrDistBw(e1.transform.position, e2.transform.position); }
    public Vector3 getMinAquariumCoords(){ return new Vector3(-dimensions.x/2, groundLevel, -dimensions.z/2); } //hard coded for aquarium default value w scale 1
    public Vector3 getMaxAquariumCoords(){ return new Vector3(dimensions.x/2, dimensions.y, dimensions.z/2); } //hard coded for aquarium default value w scale 1
    public Vector3 transformAquariumCoordsToWorldCoords(Vector3 aquariumCoords) { return transform.TransformVector(aquariumCoords); }






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
    public float getHappiness() // todo: calulate happiness bonus based on the decorations
    {
        float currHappiness = 1;
        foreach (Entity e in entities) currHappiness += e.getHappiness();
        return currHappiness;
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
