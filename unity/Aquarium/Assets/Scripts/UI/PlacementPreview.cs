using UnityEngine;
using System.Collections;

public class PlacementPreview : MonoBehaviour
{
    // public Camera camera;
    public Entity entity;
    public Entity spawnedEntity;
    public Aquarium aquarium;
    public Camera cam;
    private RaycastHit hit; 
    private Ray ray;
    private Vector3 defaultPos = new Vector3(0,0,0);


    public void init(Entity e, Aquarium a, Camera c){ //this is called first

        entity = e;
        aquarium = a;
        cam = c;


        spawnedEntity = Instantiate(e.gameObject, defaultPos, Quaternion.identity, cam.transform).GetComponent<Entity>(); //using cam.transform because transform gets an error for some reason
        spawnedEntity.initShopMode(false, true);
        //print("init");
        //print(cam);


    }


    void Start(){

    }


    // Update is called once per frame
    void Update()
    {
        //print(cam);
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            spawnedEntity.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal)*Quaternion.Euler(90f, 0f, 0f));
            
        }
        else spawnedEntity.transform.position = defaultPos;

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

//preivew BC should detect collisions w everything
//nothing should detect it, including raycasts
//this has to be set in initShopMode
//each update() it checks if BC is detecting collision and dont allow spawning if so

//so make: layer for BC to be by itself
//layer for dontAllowSpawning?