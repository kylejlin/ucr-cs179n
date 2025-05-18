using UnityEngine;
using System.Collections;

public class DragNDropPreview : MonoBehaviour
{
    // public Camera camera;
    private Entity entityPrefab; //prefab
    private Entity previewedEntity; //game object being moved around
    private Rigidbody entityRB; // if applicable (changes how it has to be moved)
    private Aquarium aquarium;
    private Camera cam;
    private RaycastHit hit;
    private Ray ray;
    private Vector3 defaultPos = new Vector3(0, 0, -20);
    private BoxCollider myBC; //BC on this game object
    private GameObject XImage;
    private Renderer Xrenderer;

    private bool isColliding; // is currently colliding with another collider in the scene
    private bool canSpawn; // is in valid position right now to spawn entity
    private bool prefab;

    /// <summary>
    /// this is called before anything else to set everything up
    /// </summary>
    /// <param name="e"> Entity to preview. Can be a prefab or an object in the scene already.</param>
    /// <param name="a"> Aquarium that it should be placed in </param>
    /// <param name="c"> main camera of scene (where the ray comes from) </param>
    /// <param name="isPrefab"> whether the entity is a prefab or an object in the scene. if its not a prefab, the same gameobject will be moved around and placed (no aquarium functions called). if its a prefab a new one will be made. </param>
    public void init(Entity e, Aquarium a, Camera c, bool isPrefab = true)
    {

        //set up all the variables

        aquarium = a;
        cam = c;
        prefab = isPrefab;
        // transform.position = Vector3.zero;
        myBC = GetComponent<BoxCollider>();
        XImage = GameObject.Find("X Image");
        Xrenderer = XImage.GetComponent<Renderer>();
        if (!e || !a || !c) Debug.LogWarning("DragNDropPreview missing references (entity, aquarium, or camera)");
        setCanSpawn(false);


        if (isPrefab)
        {
            entityPrefab = e;
            previewedEntity = Instantiate(e.gameObject, Vector3.zero, Quaternion.identity, transform).GetComponent<Entity>(); //spawn the fake entity to preview the placement
        }
        else
        {
            entityPrefab = null;
            previewedEntity = e;
        }
        Vector3 originalEntityPos = previewedEntity.transform.position;
        move(Vector3.zero, Quaternion.identity, false);
        
        entityRB = previewedEntity.GetComponent<Rigidbody>(); // doesnt work in shopmode? bc disabled?
        Bounds entityColliderBounds = previewedEntity.getAllCollidersBoundingBox(); //get its AABB for collision checks. cant nullify struct so a size 0 bounds means DNI. also doesnt work on inactive / prefab / disabled things
        if (!prefab) entityColliderBounds.center -= originalEntityPos; //for some reason Bounds require this. it does not update immediately when I set the transform above. So if the object was not at 0,0,0, the center of the AABB has the original pos added to it incorrectly
        previewedEntity.initShopMode(false, true); //it is in shop mode so it does not interfere w living real creatures
        if (!myBC || (entityColliderBounds.size == Vector3.zero) || !XImage) Debug.LogWarning("DragNDrog or entity missing components");

        if (myBC && (entityColliderBounds.size != Vector3.zero)) // we need a collider (in trigger mode so things can pass thru) to detect collisions and invalid spawining places. make it the same shape/size as the entities AABB
        {
            myBC.size = Vector3.Scale(entityColliderBounds.size, previewedEntity.transform.localScale); //i have no idea why this is needed. bounds are supposed to be in world space and it is during gamplay but not during awake?? i must be missing smth
            myBC.center = Vector3.Scale(entityColliderBounds.center, previewedEntity.transform.localScale); //IM DEADD i have no idea why this works. this is gibberish LOLLL
        }

        Vector3 temp = XImage.transform.position;
        temp.y = myBC.center.y + (myBC.size.y / 2); //this xtra step is necessary because of how transform works ig
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
            move(hit.point, Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90f, 0f, 0f), false);

            if (isColliding || !aquarium.isInBounds(hit.point) || hit.collider.GetComponent<Creature>()) //detect invalid placement (collisions, out of aquarium, or on top of a creature)
            {
                setCanSpawn(false);
            }
            else setCanSpawn(true);

            //spawn entity and dissappear if player click
            if (Input.GetMouseButtonDown(0) && canSpawn)
            {
                spawnPreviewedEntity();
            }

        }
        else move(defaultPos, Quaternion.identity, true); //else hide the preview behind the camera

        if (Input.GetMouseButtonDown(1) && prefab) endDragNDrop(); //exit without placing if they right click, although they will still have spent the money.. umm
        // if you are moving an entity you cant escape it because it causes too many problems. LOL
    }

    private void spawnPreviewedEntity()
    {
        if (prefab) aquarium.addEntity(entityPrefab, transform.position, transform.rotation); //if its a prefab child of this gamobject a new one needs to be added to the aquarium
        else previewedEntity.disableShopMode(); //else its already technically in the aquarium so just let it go free
        endDragNDrop();
    }
    public void endDragNDrop()
    {
        //if the previewed entity has not already been destroyed or spawned
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
    private void move(Vector3 position, Quaternion rotation, bool local) //moves game object and previewed eneities to global position
    {
        if (local)
        {
            transform.localPosition = position;
            transform.localRotation = rotation;
        }
        else transform.SetPositionAndRotation(position, rotation); //creature is oriented onto the surface
        XImage.transform.rotation = Quaternion.Euler(90f, 0f, 0f); //image of X always should point up
        if (entityRB)//RBs unfortunately do not follow their parent and act independently, so I have to do this 
        {
            entityRB.position = transform.position;
            entityRB.rotation = transform.rotation;
        }
        else if (!prefab) //the entity is not a child of this gameobject so must also be moved independently
        {
            previewedEntity.transform.position = transform.position;
            previewedEntity.transform.rotation = transform.rotation;
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

//preivew BC should detect collisions w everything
//nothing should detect it, including raycasts
//this has to be set in initShopMode
//each update() it checks if BC is detecting collision and dont allow spawning if so

//so make: layer for BC to be by itself
//layer for dontAllowSpawning?