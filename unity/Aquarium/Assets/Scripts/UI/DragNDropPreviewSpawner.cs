using UnityEngine;

public class PlacementPreviewSpawner : MonoBehaviour
{
    public DragNDropPreview PPRay;
    public Entity testE;
    public Aquarium testA;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPreview(testE, testA);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void startPreview(Entity entity, Aquarium aquarium)
    {
        Instantiate(PPRay, new Vector3(0,0,0), Quaternion.identity, transform).init(entity, aquarium, GetComponent<Camera>());
    }
}
