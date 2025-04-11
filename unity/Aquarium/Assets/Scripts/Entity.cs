using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName = "NoName";
    [HideInInspector]
    public int id; //id of the entity
    public int buyMoney;
    public int sellMoney;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public T FindClosest<T>() where T : new() //get all objects of one type, then check their positions and return the closest

    {
        return (default(T));
        //var objs = GameObject.FindObjectsByType<T>(FindObjectsSortMode.None);
        //if(objs.Length == 0) return default(T);
        //return objs[0];
    }
}
