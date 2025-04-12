using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName = "NoName";
    [HideInInspector]
    protected int id; //id of the entity
    private int buyMoney;
    private int sellMoney;
    private Rarity rarity;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public T FindClosest<T>() where T : Entity  //get all objects of one type, then check their positions and return the closest (excluding self)

    {
        T[] foundEntities = getAllOfType<T>();
        if (foundEntities == null) return default(T);

        T closest = default(T);

        foreach(T entity in foundEntities)
        {
            if ((getSqrDistTo(entity) < getSqrDistTo(closest) && (getSqrDistTo(entity) > 0))) // dont count yourself
            {
                closest = entity;
            }
        }
        return closest;
    }

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
        if (scaleFactor <= 0) { Debug.Log("Error: setScaleTo() scaleFactor cannot be <= 0 "); return; }
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    public float getSqrDistTo(Entity entity) { return (transform.localPosition - entity.transform.localPosition).sqrMagnitude;  }
    public int getID() { return id; }
    public int getBuyMoney() { return buyMoney; }
    public int getSellMoney() { return sellMoney; }
    public Rarity GetRarity() { return rarity; }

}
