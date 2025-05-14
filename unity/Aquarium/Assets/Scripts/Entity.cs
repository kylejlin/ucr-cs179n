using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName = "NoName";
    [HideInInspector]
    public int id; //id of the entity TYPE, set by gamemanager
    [SerializeField]
    private static double uniqueIDCount = 0; 
    private double uniqueID; //ID unique to every entity in the game
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
        uniqueID = uniqueIDCount;
        uniqueIDCount++;
        name = entityName + " " + uniqueID;
        outline = gameObject.GetComponent<Outline>();
        if(!outline) outline = gameObject.AddComponent<Outline>(); //outline script that allows the creature or decor to be outlined when player clicks on them
        setOutline(false); 
    }

    // Update is called once per frame
    void Update()
    {

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
    /// disable all colliders of this gameobject or its children. //doesnt get inactive colliders. Im not sure if it should??
    /// </summary>
    public void disableAllColliders() {
        Collider[] allColliders = GetComponentsInChildren<Collider>(); 
        foreach(Collider c in allColliders){ //go through all children colliders and disable them. They wont cause collisions or do anything
            c.enabled = false;
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

        // print(allColliders.Length);
        // print(allColliders[0]);
        // print(allColliders[0].bounds);
        // print(colliderBounds);

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
        disableAllColliders(); //dont mess w collisions or raycasts etc
        if (GetComponent<Rigidbody>()) Destroy(GetComponent<Rigidbody>()); //no physics please
    } //get overridden by child classes. Also this is permenant, reenabling an object would be difficult and might break things in Awake()

    public virtual string getCurrStats(){
        return "Name: "+entityName;
    }

    public float getSqrDistToEntity(Entity entity) { return (transform.localPosition - entity.transform.localPosition).sqrMagnitude; }
    public float getSqrDistBw(Vector3 vec1, Vector3 vec2) { return (vec1 - vec2).sqrMagnitude; }
    public void setOutline(bool enable){ outline.enabled = enable; }
    public int getID() { return id; }
    public double getUniqueID() { return uniqueID; }
    public int getBuyMoney() { return buyMoney; }
    public int getSellMoney() { return sellMoney; }
    public float getScale() { return transform.localScale.x; }
    public Rarity GetRarity() { return rarity; }
    public bool isOutlined(){ return outline.enabled; }
    
}
