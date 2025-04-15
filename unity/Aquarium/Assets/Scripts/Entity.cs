using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName = "NoName";
    [HideInInspector]
    public int id; //id of the entity
    private int buyMoney;
    private int sellMoney;
    private Rarity rarity;
    public Aquarium parentAquarium = null;
    protected double count = 0; //to count deltaTime 


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <returns> entity of type T that is closest to the caller or null if there are none found </returns>
    public T FindClosest<T>() where T : Entity  //get all objects of one type, then check their positions and return the closest (excluding self)

    {
        T[] foundEntities = getAllOfType<T>();
        if (foundEntities == null) return default(T);

        T closest = default(T);

        foreach(T entity in foundEntities)
        {
            float newDist = getSqrDistTo(entity);
            if(newDist <= 0) continue; // dont count yourself

            if(closest == default(T)) closest = entity; 
            else if ((newDist < getSqrDistTo(closest))) 
            {
                closest = entity;
            }
        }
        return closest;
    }


    /// <returns> array of type T of all the found entities, or null if none were found </returns>
    public T[] getAllOfType<T>() where T : Entity //RETURNS NULL IF EMPTY
    {
        Object[] foundObjects = FindObjectsByType(typeof(T), FindObjectsSortMode.None); //find all of the type
        if (foundObjects.Length == 0) return null;


        T[] foundEntities = new T[foundObjects.Length]; //cast to T
        for (int i = 0; i < foundObjects.Length; i++) foundEntities[i] = (T)foundObjects[i];
        return foundEntities;
    }

    protected void setScaleTo(float scaleFactor) //limited to proportional scaling only
    {
        if (scaleFactor <= 0) { Debug.LogWarning("setScaleTo() scaleFactor cannot be <= 0 "); return; }
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

    }

    public void die(){
        if(parentAquarium != null) { parentAquarium.removeEntity(this);}
        else { Debug.LogWarning("Could not find Aquarium parent"); }
        Destroy(gameObject);
    }

    public float getSqrDistTo(Entity entity) { return (transform.localPosition - entity.transform.localPosition).sqrMagnitude;  }
    public int getID() { return id; }
    public int getBuyMoney() { return buyMoney; }
    public int getSellMoney() { return sellMoney; }
    public Rarity GetRarity() { return rarity; }

}
