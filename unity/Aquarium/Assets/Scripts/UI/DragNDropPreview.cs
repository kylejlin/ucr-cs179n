using UnityEngine;
using System.Collections;

public class DragNDropPreview : MonoBehaviour
{
    // public Camera camera;
    private Entity entity; //prefab
    private Entity spawnedEntity;
    private Rigidbody entityRB;
    private Aquarium aquarium;
    private Camera cam;
    private RaycastHit hit; 
    private Ray ray;
    private Vector3 defaultPos = new Vector3(0,0,-20);
    private BoxCollider myBC;
    private GameObject XImage;
    private Renderer Xrenderer;

    private bool isColliding;
    private bool canSpawn;


    public void init(Entity e, Aquarium a, Camera c){ //this is called first to set everything up

        //set up all the variables

        entity = e;
        aquarium = a;
        cam = c;
        transform.position = new Vector3(0,0,0);
        myBC = GetComponent<BoxCollider>();
        XImage = GameObject.Find("X Image");
        Xrenderer = XImage.GetComponent<Renderer>(); 
        if (!e || !a || !c) Debug.LogWarning("DragNDropPreview missing references (entity, aquarium, or camera)");


        setCanSpawn(false);
        spawnedEntity = Instantiate(e.gameObject, new Vector3(0, 0, 0), Quaternion.identity, transform).GetComponent<Entity>(); //spawn the fake entity to preview the placement
        spawnedEntity.transform.position = new Vector3(0,0,0);
        Bounds entityColliderBounds = spawnedEntity.getAllCollidersBoundingBox(); //get its AABB for collision checks. cant nullify struct so a size 0 bounds means DNI. also doesnt work on inactive / prefab / disabled things
        entityRB = spawnedEntity.GetComponent<Rigidbody>();
        spawnedEntity.initShopMode(false, true); //it is in shop mode so it does not interfere w living real creatures
        if (!myBC || (entityColliderBounds.size == new Vector3(0,0,0)) || !XImage) Debug.LogWarning("DragNDrog or entity missing components");

        if(myBC && (entityColliderBounds.size != new Vector3(0,0,0))) // we need a collider (in trigger mode so things can pass thru) to detect collisions and invalid spawining places. make it the same shape/size as the entities AABB
        {
            myBC.size = Vector3.Scale(entityColliderBounds.size, spawnedEntity.transform.localScale); //i have no idea why this is needed. bounds are supposed to be in world space and it is during gamplay but not during awake?? i must be missing smth
            myBC.center = Vector3.Scale(entityColliderBounds.center, spawnedEntity.transform.localScale); //IM DEADD i have no idea why this works. this is gibberish LOLLL
        }

        Vector3 temp = XImage.transform.position;
        temp.y = myBC.center.y+ (myBC.size.y/2); //this xtra step is necessary because of how transform works ig
        XImage.transform.position = temp; //move the X to be right on top of the entity

        //How this works:
        //spawn entity
        //get the bounding box of its colliders only
        //turn on shopmode, so all of the entitie's colliders will be disabled and its RB will be deleted if it has one 
        //change the collider on this gameObject to match the size and placement of the entity's colliders so it can detect collisions (invalid spawning places)

        //Also:
        //this object is in IgnoreRaycast Layer so the raycast wont hit it
        //raycast wont hit the entity because it is in shopmode and colliders are disabled
        //nothing will hit this because its collider is in trigger mode
        //it still gets collision events though because it has a rigidbody. The RB is kinematic though (basically turned off)

    }

    // Update is called once per frame
    void Update()
    {

        ray = cam.ScreenPointToRay(Input.mousePosition); //ray from player POV
        if (Physics.Raycast(ray, out hit)) //if it hits anything
        {
            //move to mouse position & rotate
            // if(entityRB) entityRB.Move(hit.point,  Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90f, 0f, 0f));
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
                endDragNDrop();
            }

        }
        else transform.localPosition = defaultPos; //else hide the preview behind the camera

        if (Input.GetMouseButtonDown(1)) endDragNDrop(); //exit without placing if they right click, although they will still have spent the money.. umm

    }

    public void endDragNDrop(){
        Destroy(gameObject);
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