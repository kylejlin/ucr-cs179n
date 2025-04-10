using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName = "NoName";
    public int value = 10; //value when bought and sold



    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    protected void setScaleTo(float scaleFactor) //limited to proportional scaling only
    {
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
}
