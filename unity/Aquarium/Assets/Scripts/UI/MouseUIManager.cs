using UnityEngine;

public class MouseUIManager : MonoBehaviour
{
    public DragNDropPreview PPRay; //prefab
    public StatsPopup statsPopup; //prefab
    private DragNDropPreview currentPPRay; //curently spawned preview

    private int entityLayerMask = 1 << 15; //mask so mouse ray can only hit entities
   
    private Camera cam;
    private Ray ray; //ray from the mouse
    private RaycastHit hit; 
    private Entity selectedEntity;
    void Start()
    {
       cam = GetComponent<Camera>();
       if(!PPRay) Debug.LogWarning("No DragNDropPreview set");
       if(!cam) Debug.LogWarning("No camera found");
    }

    /// <summary>
    /// start a dragNDrop session. Will show a preview of what the entity is and where it will spawn. restricts it to aquarium and doesnt allow collisions
    /// </summary>
    public void startPreview(Entity entity, Aquarium aquarium) //called by gameManager when adding smth from the shop
    {
        if (currentPPRay) Debug.LogWarning("Multiple DragNDrop previews present (there should only be one at any time)");
        currentPPRay = Instantiate(PPRay, new Vector3(0,0,0), Quaternion.identity, transform);
        currentPPRay.init(entity, aquarium, cam);
    }

    void Update(){
        if (Input.GetMouseButtonDown(0)){ //if player clicks check for entity at that spot
            ray = cam.ScreenPointToRay(Input.mousePosition); //ray from player POV
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, entityLayerMask)) //if it hits an entity
            {
                selectedEntity = hit.collider.GetComponent<Entity>();
                if(selectedEntity) print(selectedEntity.getCurrStats());
            }
        }
        
    }


}
