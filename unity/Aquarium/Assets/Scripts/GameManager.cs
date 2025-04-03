using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MobileCreature trilobite;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MobileCreature newTrilobite = Instantiate(trilobite, new Vector3(0, 0, 0), Quaternion.identity); //spawns trilobite at origin and not rotated
        newTrilobite.entityName = "Bob";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
