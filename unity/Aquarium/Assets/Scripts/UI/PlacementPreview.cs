using UnityEngine;
using System.Collections;

public class PlacementPreview : MonoBehaviour
{
    // public Camera camera;
    private Entity entity;
    private Entity spawnedEntity;
    private Aquarium aquarium;
    private Camera cam;
    private RaycastHit hit; 
    private Ray ray;
    private Vector3 defaultPos = new Vector3(0,0,0);

    void Start(){
        // this.enabled = false; //dont call update until init() is run
    }

    public void init(Entity e, Aquarium a, Camera c){

        entity = e;
        aquarium = a;
        cam = c;
        // this.enabled = true;

        spawnedEntity = Instantiate(e.gameObject, defaultPos, Quaternion.identity, cam.transform).GetComponent<Entity>(); //using cam.transform because transform gets an error for some reason
        spawnedEntity.initShopMode(false, true);
        print("preview spawned");


    }

    // Update is called once per frame
    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;
            // aquarium.addEntity(entity, hit.point, Quaternion.identity);   
        }

    }
}


/*previewer in camera
shopUI calls camera and passes prefab to spawn and current aquarium
camera spawns preview rays 
player clicks, rays call aquarium addEntity, deletes
should collide with everything, but has to know what positions are valid (not on the walls, has enough space on the ground)

*/

//object is created / INstaniated
//Awake is called
//functions are called on it (ex. initShopMode)
//THEN start is called but only if it is enabled
//then update is called