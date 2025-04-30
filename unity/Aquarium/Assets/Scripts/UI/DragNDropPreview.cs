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
    private Vector3 defaultPos = new Vector3(0,0,0);
    private BoxCollider myBC;
    private BoxCollider entityBC;
    private GameObject XImage;

    private bool isColliding;
    private bool canSpawn;


    public void init(Entity e, Aquarium a, Camera c){ //this is called first

        entity = e;
        aquarium = a;
        cam = c;
        transform.position = defaultPos;

        myBC = GetComponent<BoxCollider>();
        entityBC = entity.GetComponent<BoxCollider>();
        XImage = GameObject.Find("X Image");

        if (!e || !a || !c) Debug.LogWarning("DragNDropPreview missing references (entity, aquarium, or camera)");
        if (!myBC || !entityBC || !XImage) Debug.LogWarning("DragNDrog or entity missing components");


        setCanSpawn(false);
        spawnedEntity = Instantiate(e.gameObject, new Vector3(0, 0, 0), Quaternion.identity, transform).GetComponent<Entity>(); //using cam.transform because transform gets an error for some reason
        spawnedEntity.initShopMode(false, true);

        if(myBC && entityBC)
        {
            myBC.size = Vector3.Scale(entityBC.size, spawnedEntity.transform.localScale);
            myBC.center  = Vector3.Scale(entityBC.center, spawnedEntity.transform.localScale);
        }


    }


    void Start(){

    }


    // Update is called once per frame
    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //move to mouse position & rotate
            transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90f, 0f, 0f));
            XImage.transform.rotation = Quaternion.Euler(90f, 0f, 0f); //image always should point up
            if (isColliding || hit.collider.CompareTag("DontAllowSpawn")) 
            { 
                setCanSpawn(false);
            }
            else setCanSpawn(true);
                // print(hit.collider.CompareTag("DontAllowSpawn"));

            //spawn guy and dissappear if they click
            if (Input.GetMouseButtonDown(0) && canSpawn)
            {
                aquarium.addEntity(entity, transform.position, transform.rotation);
                Destroy(gameObject);
            }

        }
        else transform.position = defaultPos;
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
        if (XImage) XImage.GetComponent<Renderer>().enabled = !enable; //hide the X
        else Debug.LogWarning("Sprite Renderer for invalid placement indication not found");
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