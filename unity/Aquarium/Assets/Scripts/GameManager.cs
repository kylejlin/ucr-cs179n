using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ImmobileCreature algea; //have to assign these manually in the editor
    public MobileCreature trilobite;
    public Entity trilobite1;
    public Aquarium aquarium; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // aquarium = GameObject.Find("Aquarium"); // this will have to be changed: it should be spawned, not referenced, but this is convenient for the MVP
        aquarium.addEntity(trilobite, new Vector3(1,1,0), Quaternion.identity);
        aquarium.addEntity(algea, new Vector3(1, 1, 1), Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
