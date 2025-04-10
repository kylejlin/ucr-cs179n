using UnityEngine;
using System.Collections.Generic; //list and dictionary definition


public class Aquarium : MonoBehaviour
{
    public List<Entity> entities = new List<Entity>(); // all creatures (and objects?) within the tank
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 dimensions = new Vector3(50, 40, 50); // hard coded to fit basic aquarium, should be changable later (arbitrary water level of 40 cm)
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addEntity(Entity newEntity, Vector3 position, Quaternion rotation)
    {
       entities.Add(Instantiate(newEntity, position, rotation, transform)); //add entity as child of this Aquarium
    }
}
