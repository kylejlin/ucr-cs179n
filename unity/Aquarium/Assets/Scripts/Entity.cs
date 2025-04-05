using UnityEngine;

public class Entity : MonoBehaviour
{
    //add code for rigidbody? all entities should have a rigidbody?
    public float hunger = 0;
    public string entityName = "NoName";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public T FindClosest<T>() where T : new()
    {
        var objs = GameObject.FindObjectsByType<T>(FindObjectsSortMode.None);
        if(objs.Length == 0) return default(T);
        return objs[0];
    }
}
