using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName = "NoName";
    [HideInInspector]
    public int id; //id of the entity
    [SerializeField]
    private int buyMoney;
    [SerializeField]
    private int sellMoney;
    [SerializeField]
    private Rarity rarity;
    protected bool bottomDweller = true; //should it spawn on the bottom of the tank. True for all decorations. For creatures, its true if it is immobile or only walks along the bottom, else false.

    public Aquarium parentAquarium = null;
    protected double count = 0; //to count deltaTime 
    public bool shopMode = false; //true if this gameobject is being displayed in UI and so should spawn as an adult and not Update() (frozen, don't interact) 
    protected void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <returns> entity of type T that is closest to the caller (excludes self) or null if there are none found </returns>
    public T FindClosest<T>() where T : Entity  //get all objects of one type, then check their positions and return the closest (excluding self)

    {
        T[] foundEntities = getAllOfType<T>();
        if (foundEntities == null) return default(T);

        T closest = default(T);

        foreach (T entity in foundEntities)
        {
            float newDist = getSqrDistToEntity(entity);
            if(newDist <= 0) continue; // dont count yourself

            if(closest == default(T)) closest = entity; 
            else if ((newDist < getSqrDistToEntity(closest))) 
            {
                closest = entity;
            }
        }
        return closest;
    }

    /// <returns> entity of type T that is closest to Position or null if there are none found </returns>
    public T FindClosest<T>(Vector3 position) where T : Entity  //get all objects of one type, then check their positions and return the closest (excluding self)

    {
        T[] foundEntities = getAllOfType<T>();
        if (foundEntities == null) return default(T);

        T closest = default(T);

        foreach (T entity in foundEntities)
        {
            float newDist = getSqrDistBw(entity.transform.localPosition, position);

            if (closest == default(T)) closest = entity;
            else if ((newDist < getSqrDistBw(closest.transform.localPosition, position)))
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
    
    public float getSqrDistToEntity(Entity entity) { return (transform.localPosition - entity.transform.localPosition).sqrMagnitude; }
    public float getSqrDistBw(Vector3 vec1, Vector3 vec2) { return (vec1 - vec2).sqrMagnitude; }
    public int getID() { return id; }
    public int getBuyMoney() { return buyMoney; }
    public int getSellMoney() { return sellMoney; }
    public float getScale() { return transform.localScale.x; }
    public Rarity GetRarity() { return rarity; }
    public virtual void initShopMode(bool asAdult = true, bool changeMaturity = true) { this.enabled = false; shopMode = true; } //get overridden by child classes
    public virtual void disableShopMode() {this.enabled = true; shopMode = false;}

}
