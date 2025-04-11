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

    public void addEntity(GameObject newEntity, Vector3 position, Quaternion rotation)
    {
        entities.Add(Instantiate(newEntity, position, rotation).GetComponent<Entity>());
        print("Number of entities in tank: " + entities.Count);
    }
    public void addEntity(GameObject newEntity)
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-dimensions.x / 2, dimensions.x / 2),
            Random.Range(0, dimensions.y),
            Random.Range(-dimensions.z / 2, dimensions.z / 2)
        );

        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(0, 360),
            Random.Range(0, 360),
            Random.Range(0, 360)
        );

        entities.Add(Instantiate(newEntity, randomPosition, randomRotation, gameObject.transform).GetComponent<Entity>());
        print("Number of entities in tank: " + entities.Count);
    }

    public int calcCoin()
    {
        return 10; // todo: calculate the coins based on the number of creatures and decorations
    }

    public int getHunger()
    {
        return 20; // todo: calulate hunger percentage based on trilobites
    }
    public float getHappiness() // todo: calulate happiness bonus based on the decorations
    {
        return 0.10f;
    }
    public int getAlgaesHealth() // todo: calulate algaes health based on algaes
    {
        return 10;
    }
    public int getTrilobitesConsume() // todo: calulate total consume based on algaes
    {
        return 10;
    }

}
