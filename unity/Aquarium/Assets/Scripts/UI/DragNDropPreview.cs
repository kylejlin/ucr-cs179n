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
    private GameObject XImage;
    private Renderer Xrenderer;

    private bool isColliding;
    private bool canSpawn;


    public void init(Entity e, Aquarium a, Camera c){ //this is called first to set everything up

        //set up all the variables

        entity = e;
        aquarium = a;
        cam = c;
        transform.position = defaultPos;
        myBC = GetComponent<BoxCollider>();
        XImage = GameObject.Find("X Image");
        Xrenderer = XImage.GetComponent<Renderer>(); 
        if (!e || !a || !c) Debug.LogWarning("DragNDropPreview missing references (entity, aquarium, or camera)");


        setCanSpawn(false);
        spawnedEntity = Instantiate(e.gameObject, new Vector3(0, 0, 0), Quaternion.identity, transform).GetComponent<Entity>(); //spawn the fake entity to preview the placement
        Bounds entityColliderBounds = spawnedEntity.getAllCollidersBoundingBox(); //get its AABB for collision checks. cant nullify struct so a size 0 bounds means DNI. also doesnt work on inactive / prefab / disabled things
        spawnedEntity.transform.localPosition = new Vector3(0,0,0);
        spawnedEntity.initShopMode(false, true); //it is in shop mode so it does not interfere w living real creatures
        if (!myBC || (entityColliderBounds.size == new Vector3(0,0,0)) || !XImage) Debug.LogWarning("DragNDrog or entity missing components");

        if(myBC && (entityColliderBounds.size != new Vector3(0,0,0))) // we need a collider (in trigger mode so things can pass thru) to detect collisions and invalid spawining places. make it the same shape/size as the entities AABB
        {
            myBC.size = entityColliderBounds.size; 
            myBC.center = entityColliderBounds.center - transform.position; //bounds coords are in worldspace and myBC is in local space, so have to fix it
        }

        //need:
        //cant just delete BC because it may have more Cs
        //need ray to go thru the prview (set layer to smth else, use ray mask?)
        //need to detect when preview is colliding w other things (make all Cs triggers in initShopmode?) (cant just disable them) (maybe I can? for the raycast?)
        //in entity: disableAllColliders, get boundingboxofcolliders, make Cs triggers
        //fast(er) solution: get boundingboxofcolliders in entity. use that for preview BC

        //need: preview to know when its colliding
        //ray to not hit entities BC

        //plan:
        //init entity
        //get its C BB
        //its RB will be deleted bc shopmode
        //make BC 
        //disableAllColliders (can i do that in shopmode?) V
        //cast ray: wont hit this object because its is in IgnoreRaycast layer
        //wont hit preview bc disabled

        
        
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