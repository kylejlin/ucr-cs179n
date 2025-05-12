using UnityEngine;
using System.Collections;

public class DragNDropPreview : MonoBehaviour
{
    // public Camera camera;
    private Entity entity; //prefab
    private Entity spawnedEntity;
    private Aquarium aquarium;
    private Camera cam;
    private RaycastHit hit; 
    private Ray ray;
    private Vector3 defaultPos = new Vector3(0,0,-20);
    private BoxCollider myBC;
    private BoxCollider entityBC;
    private GameObject XImage;
    private Renderer Xrenderer;

    private bool isColliding;
    private bool canSpawn;


    public void init(Entity e, Aquarium a, Camera c){ //this is called first to set everything up

        entity = e;
        aquarium = a;
        cam = c;
        transform.position = defaultPos;

        myBC = GetComponent<BoxCollider>();
        entityBC = entity.GetComponent<BoxCollider>();
        XImage = GameObject.Find("X Image");
        Xrenderer = XImage.GetComponent<Renderer>(); //set up all the variables

        if (!e || !a || !c) Debug.LogWarning("DragNDropPreview missing references (entity, aquarium, or camera)");
        if (!myBC || !entityBC || !XImage) Debug.LogWarning("DragNDrog or entity missing components");


        setCanSpawn(false);
        spawnedEntity = Instantiate(e.gameObject, new Vector3(0, 0, 0), Quaternion.identity, transform).GetComponent<Entity>(); //spawn the fake entity to preview the placement
        spawnedEntity.transform.localPosition = new Vector3(0,0,0);
        spawnedEntity.initShopMode(false, true); //it is in shop mode so it does not interfere w living real creatures

        if(myBC && entityBC) //it will not have a collider anymore bc shop mode. but we need a collider (in trigger mode so things can pass thru) to detect collisions and invalid spawining places. so make a new one the same size and location
        {
            myBC.size = Vector3.Scale(entityBC.size, spawnedEntity.transform.localScale);
            myBC.center  = Vector3.Scale(entityBC.center, spawnedEntity.transform.localScale);
        }


    }

    // Update is called once per frame
    void Update()
    {

        ray = cam.ScreenPointToRay(Input.mousePosition); //ray from player POV
        if (Physics.Raycast(ray, out hit)) //if it hits anything
        {
            //move to mouse position & rotate
            transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90f, 0f, 0f)); //creature is oriented onto the surface
            XImage.transform.rotation = Quaternion.Euler(90f, 0f, 0f); //image of X always should point up
            if (isColliding || !aquarium.isInBounds(hit.point) || hit.collider.GetComponent<Creature>()) //detect invalid placement (collisions, out of aquarium, or on top of a creature)
            { 
                setCanSpawn(false);
            }
            else setCanSpawn(true);


            //spawn entity and dissappear if player click
            if (Input.GetMouseButtonDown(0) && canSpawn)
            {
                aquarium.addEntity(entity, transform.position, transform.rotation);
                Destroy(gameObject);
            }

        }
        else transform.localPosition = defaultPos; //else hide the preview behind the camera
    }

    private void OnTriggerEnter(Collider other)
    {
        isColliding = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isColliding = false;
    }
    private void setCanSpawn(bool enable)
    {
        if (Xrenderer) Xrenderer.enabled = !enable; //X shows when enable is false
        canSpawn = enable;
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