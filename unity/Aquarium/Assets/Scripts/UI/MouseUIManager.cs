using UnityEngine;

public class MouseUIManager : MonoBehaviour
{
    public DragNDropPreview PPRay; //prefab
    public StatsPopup statsPopup; //prefab
    private DragNDropPreview currentPPRay; //curently spawned preview
    private StatsPopup currentStatsPopup; //currently spawned stats popup

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

        currentStatsPopup = Instantiate(statsPopup, new Vector3(0,0,0), Quaternion.identity, transform);
        currentStatsPopup.closePopup();
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

    public void startPopup(Entity entity){
        currentStatsPopup.startPopup(entity);
    }
    public void closePopup(){
        currentStatsPopup.closePopup();
    }

    void Update(){
        if (Input.GetMouseButtonDown(0)){ //if player left clicks check for entity at that spot
            ray = cam.ScreenPointToRay(Input.mousePosition); //ray from player POV
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, entityLayerMask)) //if it hits an entity
            {
                selectedEntity = hit.collider.GetComponent<Entity>();
                if(selectedEntity) startPopup(selectedEntity); //double check to make sure the object has the entity script
                else closePopup();
            }
            else closePopup();

        }

        if(Input.GetMouseButtonDown(1)) closePopup(); //if player right clicks exit out of the selections
        
    }


}
