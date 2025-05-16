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
    [SerializeField]
    protected double count = 0; //to count deltaTime 
    public bool shopMode = false; //true if this gameobject is being displayed in UI and so should spawn as an adult and not Update() (frozen, don't interact) 

    private Outline outline;
    public virtual void Awake()
    {
        SetLayerRecursively(transform, 15); //set to Entity layer for raycast masking
        if(name == "NoName") name = entityName + " " + id;
        outline = gameObject.GetComponent<Outline>();
        if(!outline) outline = gameObject.AddComponent<Outline>(); //outline script that allows the creature or decor to be outlined when player clicks on them
        setOutline(false); 
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <param name="global"> if true search entire scene, if false search only the same aquarium</param>
    /// <returns> entity of type T that is closest to the caller (excludes self) or null if there are none found </returns>
    public T FindClosest<T>(bool global = false) where T : Entity  //get all objects of one type, then check their positions and return the closest (excluding self)

    {
        T[] foundEntities = getAllOfType<T>(global);
        if (foundEntities == null) return default(T);

        T closest = default(T);

        foreach (T entity in foundEntities)
        {
            float newDist = getSqrDistToEntity(entity);
            if (newDist <= 0) continue; // dont count yourself

            if (closest == default(T)) closest = entity;
            else if ((newDist < getSqrDistToEntity(closest)))
            {
                closest = entity;
            }
        }
        return closest;
    }

    /// <param name="global"> if true search entire scene, if false search only the same aquarium</param>
    /// <returns> entity of type T that is closest to Position (does not exclude self) or null if there are none found </returns>
    public T FindClosest<T>(Vector3 position, bool global = false) where T : Entity  //get all objects of one type, then check their positions and return the closest (excluding self)

    {
        T[] foundEntities = getAllOfType<T>(global);
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


    /// <param name="global"> if true search entire scene, if false search only the same aquarium</param>
    /// <returns> array of type T of all the found entities, or null if none were found </returns>
    public T[] getAllOfType<T>(bool global = false) where T : Entity //RETURNS NULL IF EMPTY
    {
        if(global){ //replace w getcompinchildren no casting needed
            Object[] foundObjects = FindObjectsByType(typeof(T), FindObjectsSortMode.None); //find all of the type
            if (foundObjects.Length == 0) return null;


            T[] foundEntities = new T[foundObjects.Length]; //cast to T
            for (int i = 0; i < foundObjects.Length; i++) foundEntities[i] = (T)foundObjects[i];
            return foundEntities;}
        else {
            T[] foundComponents = parentAquarium.GetComponentsInChildren<T>();
            return foundComponents;
        }
    }

    protected void setScaleTo(float scaleFactor) //limited to proportional scaling only
    {
        if (scaleFactor <= 0) { Debug.LogWarning("setScaleTo() scaleFactor cannot be <= 0 "); return; }
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

    }

/// <summary>
/// destroy the gameobject and let the aquarium know
/// </summary>
    public void die()
    {
        if (parentAquarium != null) { parentAquarium.removeEntity(this); }
        else { Debug.LogWarning("Could not find Aquarium parent"); }
        Destroy(gameObject);
    }
    
    public void SetLayerRecursively(Transform obj, int newLayer)
    {
        obj.gameObject.layer = newLayer;
        foreach (Transform child in obj)
        {
            SetLayerRecursively(child, newLayer);
        }
    }
    
    /// <summary>
    /// disable/enable all colliders of this gameobject or its children. //doesnt get inactive colliders. Im not sure if it should??
    /// </summary>
    public void enableAllColliders(bool enable) {
        Collider[] allColliders = GetComponentsInChildren<Collider>(); 
        foreach(Collider c in allColliders){
            c.enabled = enable;
        }
    }
    /// <summary>
    /// ONLY WORKS IF GAMEOBJECT AND COLLIDERS ARE ENABLED AND ACTIVE. get the axis aligned BB of all the colliders on this gameobject or its children. //doesnt get inactive colliders. Im not sure if it should??
    /// </summary>
    /// <returns> Bounds struct of AABB. will return a Bounds with center 0,0,0 and size 0,0,0 if there are no colliders, because structs cant be null</returns>
    public Bounds getAllCollidersBoundingBox(){
        if(!gameObject.activeInHierarchy) { Debug.LogWarning("Object Inactive. No bounds"); return new Bounds(new Vector3(0,0,0), new Vector3(0,0,0));}
        Collider[] allColliders = GetComponentsInChildren<Collider>(); 
        if(allColliders.Length==0) { Debug.LogWarning("No colliders"); return new Bounds(new Vector3(0,0,0), new Vector3(0,0,0));}
        Bounds colliderBounds = allColliders[0].bounds;

        foreach(Collider c in allColliders){ //go through all children colliders and expand the bounds to hold them all
            colliderBounds.Encapsulate(c.bounds.min);
            colliderBounds.Encapsulate(c.bounds.max);
        }


        return colliderBounds;

    }
    /// <summary>
    /// make it fake and noninteractive and invisible to other creatures. For display purposes like in shop or dragndrop or UI etc
    /// </summary>
    /// <param name="asAdult"> set self to be an adult or a baby as they would naturally spawn</param>
    /// <param name="changeMaturity"> should this set the maturity. if false, isadult doesnt matter</param>
    public virtual void initShopMode(bool asAdult = true, bool changeMaturity = true) { 
        this.enabled = false;  //no update()
        shopMode = true;
        enableAllColliders(false); //dont mess w collisions or raycasts etc
        
        Rigidbody RB = GetComponent<Rigidbody>();
        if (RB) {
            RB.isKinematic = true;
            RB.detectCollisions = false;
            print(RB);
        } //no physics please
    } //get overridden by child classes.

    public virtual void disableShopMode(){
        this.enabled = true;
        shopMode = false;
        enableAllColliders(true);

        Rigidbody RB = GetComponent<Rigidbody>();
        if (RB) RB.isKinematic = false; //physics please
    }

    public virtual string getCurrStats(){
        return "Name: "+entityName;
    }

    public float getSqrDistToEntity(Entity entity) { return (transform.localPosition - entity.transform.localPosition).sqrMagnitude; }
    public float getSqrDistBw(Vector3 vec1, Vector3 vec2) { return (vec1 - vec2).sqrMagnitude; }
    public void setOutline(bool enable){ outline.enabled = enable; }
    public int getID() { return id; }
    public int getBuyMoney() { return buyMoney; }
    public int getSellMoney() { return sellMoney; }
    public float getScale() { return transform.localScale.x; }
    public Rarity GetRarity() { return rarity; }
    public bool isOutlined(){ return outline.enabled; }
    
}
