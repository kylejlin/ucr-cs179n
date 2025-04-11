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


    public T FindClosest<T>() where T : Entity  //get all objects of one type, then check their positions and return the closest

    {
        T[] foundEntities = getAllOfType<T>();
        if (foundEntities == null) return default(T);
        return foundEntities[0]; 
    }

    public T[] getAllOfType<T>() where T : Entity //RETURNS NULL IF EMPTY
    {
        Object[] foundObjects = FindObjectsByType(typeof(T), FindObjectsSortMode.None); //find all of the type
        if (foundObjects.Length == 0) return null;


        T[] foundEntities = new T[foundObjects.Length]; //cast to T
        for(int i = 0; i < foundObjects.Length; i++) foundEntities[i] = (T)foundObjects[i];
        return foundEntities;
    }

    protected void setScaleTo(float scaleFactor) //limited to proportional scaling only
    {
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
}
