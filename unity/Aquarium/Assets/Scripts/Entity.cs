using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] public int id; //id of the entity type, set in gamemanager
    [SerializeField] public string nameBase = "Entity"; //default name (name of the species) set in prefab editor
    private static double uniqueIDCount = 0;
    [SerializeField] private double uniqueID = -1; //unique ID of this gameobject only. 
    [SerializeField] private float buyMoney;
    [SerializeField] private float sellMoney;
    [SerializeField] private Rarity rarity;
    [SerializeField] public Aquarium parentAquarium = null;
    [SerializeField] private Bounds AABB; // set on prefab in Gamemanager when the game starts. Applies to unrotated, unscaled prefab at 0,0,0. This seems janky but I couldn't find a better way because Bounds/Colliders dont update with the transform immediately which leads to a host of problems
    [SerializeField] protected bool shopMode = false; //true if this gameobject is being displayed in UI and so should spawn as an adult and not Update() (frozen, don't interact) 
    protected double count = 0; //to count deltaTime 

    [SerializeField]
    private string description;
    private Outline outline;
    public virtual void Awake()
    {
        SetLayerRecursively(transform, 15); //set to Entity layer for raycast masking
        // if (uniqueID == -1) 
        // {
            name = nameBase + " " + uniqueIDCount;
            uniqueID = uniqueIDCount;
            uniqueIDCount++;
        // }
        outline = gameObject.GetComponent<Outline>();
        if (!outline) outline = gameObject.AddComponent<Outline>(); //outline script that allows the creature or decor to be outlined when player clicks on them
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
    /// disable/enable all colliders of this gameobject or its children. //doesnt get inactive colliders. Im not sure if it should??
    /// </summary>
    public void enableAllColliders(bool enable)
    {
        Collider[] allColliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in allColliders)
        {
            c.enabled = enable;
        }
    }
    /// <summary>
    /// ONLY WORKS IF GAMEOBJECT AND COLLIDERS ARE ENABLED AND ACTIVE. get the axis aligned BB of all the colliders on this gameobject or its children. //doesnt get inactive colliders. Im not sure if it should??
    /// </summary>
    /// <returns> Bounds struct of AABB. will return a Bounds with center 0,0,0 and size 0,0,0 if there are no colliders, because structs cant be null</returns>
    public Bounds getAllCollidersBoundingBox()
    {
        if (!gameObject.activeInHierarchy) { Debug.LogWarning("Object Inactive. No bounds"); return new Bounds(new Vector3(0, 0, 0), new Vector3(0, 0, 0)); }
        Collider[] allColliders = GetComponentsInChildren<Collider>();
        if (allColliders.Length == 0) { Debug.LogWarning("No colliders"); return new Bounds(new Vector3(0, 0, 0), new Vector3(0, 0, 0)); }
        Bounds colliderBounds = allColliders[0].bounds;

        foreach (Collider c in allColliders)
        { //go through all children colliders and expand the bounds to hold them all
            colliderBounds.Encapsulate(c.bounds.min);
            colliderBounds.Encapsulate(c.bounds.max);
        }

        return colliderBounds;

    }
    public Vector3 getClosestPointOnColliders(Vector3 position)
    {
        Collider[] allColliders = GetComponentsInChildren<Collider>();
        if (allColliders.Length == 0) { Debug.LogWarning("No colliders, returning original position"); return position; }
        Vector3 closest = allColliders[0].ClosestPoint(position);
        Vector3 temp;

        foreach (Collider c in allColliders)
        {
            if (!c.enabled) Debug.LogWarning("Collider disabled, will return original position");
            temp = c.ClosestPoint(position);
            if ((temp - position).sqrMagnitude < closest.sqrMagnitude) closest = temp;
        }

        return closest;

    }
    /// <summary>
    /// make it fake and noninteractive and invisible to other creatures. For display purposes like in shop or dragndrop or UI etc
    /// </summary>
    /// <param name="asAdult"> set self to be an adult or a baby as they would naturally spawn</param>
    /// <param name="changeMaturity"> should this set the maturity. if false, isadult doesnt matter</param>
    public virtual void initShopMode(bool asAdult = true, bool changeMaturity = true)
    {
        this.enabled = false;  //no update()
        shopMode = true;
        enableAllColliders(false); //dont mess w collisions or raycasts etc

        Rigidbody RB = GetComponent<Rigidbody>();
        if (RB) RB.isKinematic = true; //no physics please
    } //get overridden by child classes.

    public virtual void disableShopMode()
    {
        this.enabled = true;
        shopMode = false;
        enableAllColliders(true);

        Rigidbody RB = GetComponent<Rigidbody>();
        if (RB) RB.isKinematic = false; //physics please
    }

    public virtual string getCurrStats()
    {
        return "Name: " + name;
    }
    public string getShopStats()
    {
        return "Name: " + name + "\n" +
            "Description: " + description + "\n" +
            "Buy Price: " + buyMoney + "\n" +
            "Sell Price: " + sellMoney;
    }
    public float getSqrDistToEntity(Entity entity) { return (transform.localPosition - entity.transform.localPosition).sqrMagnitude; }
    public float getSqrDistBw(Vector3 vec1, Vector3 vec2) { return (vec1 - vec2).sqrMagnitude; }
    public void setOutline(bool enable) { outline.enabled = enable; }
    public int getID() { return id; }
    public double getUniqueID() { return uniqueID; }
    public float getBuyMoney() { return buyMoney; }
    public float getSellMoney() { return sellMoney; }
    public float getScale() { return transform.localScale.x; }
    public Rarity GetRarity() { return rarity; }
    public bool isOutlined() { return outline.enabled; }
    public bool isShopMode() { return shopMode; }
    public void setAABB(Bounds newAABB) { AABB = newAABB; }
    public Bounds getAABB() { return AABB; }
    public virtual float calcMoneyBonus()
    {
        switch (GetRarity())
        {
            case Rarity.Common:
                return 1;
            case Rarity.Rare:
                return 3;
            case Rarity.Epic:
                return 7;
            default:
                return 1;
        }
    }

    public virtual float getHappiness()
    {
        return 0;
    }
    
}
