using UnityEngine;
using UnityEngine.UI;

public class MouseUIManager : MonoBehaviour
{
    public DragNDropPreview PPRay; //prefab
    public StatsPopup statsPopup; //prefab
    private DragNDropPreview currentPPRay; //curently spawned preview
    private StatsPopup currentStatsPopup; //currently spawned stats popup
    private Button moveButton; 
    private Button removeButton; 

    private int entityLayerMask = 1 << 15; //mask so mouse ray can only hit entities

    private GameObject gameUI;
    //checking the state of this to determine whether or not the shop is open. This is kinda bad imo, if we continue work on this we should move this whole class under some bigger UI manager w the shop
    private Camera cam;
    private Ray ray; //ray from the mouse
    private RaycastHit hit;
    private Entity selectedEntity;
    void Start()
    {
        cam = GetComponent<Camera>();
        gameUI = GameObject.Find("/UI/GameUI");
        if (!gameUI) Debug.LogWarning("Could not find game UI");
        if (!PPRay) Debug.LogWarning("No DragNDropPreview set");
        if (!cam) Debug.LogWarning("No camera found");

        currentStatsPopup = Instantiate(statsPopup, new Vector3(0, 0, 0), Quaternion.identity, transform);
        currentStatsPopup.closePopup();
        moveButton = currentStatsPopup.moveButton;
        removeButton = currentStatsPopup.removeButton;

        moveButton.onClick.AddListener(() => moveEntity());
        removeButton.onClick.AddListener(() => removeEntity());
    }

    /// <summary>
    /// start a dragNDrop session. Will show a preview of what the entity is and where it will spawn. restricts it to aquarium and doesnt allow collisions
    /// </summary>
    public void startPreview(Entity entity, Aquarium aquarium) //called by gameManager when adding smth from the shop
    {
        if (currentPPRay) Debug.LogWarning("Multiple DragNDrop previews present (there should only be one at any time)");
        currentPPRay = Instantiate(PPRay, new Vector3(0, 0, 0), Quaternion.identity, transform);
        currentPPRay.init(entity, aquarium, cam);
        closePopup(); // I think it would be weird if both were happening at the same time
    }

    public void startPopup(Entity entity)
    {
        currentStatsPopup.startPopup(entity);
    }
    public void closePopup()
    {
        currentStatsPopup.closePopup();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && (!currentPPRay) && (gameUI) && (gameUI.activeInHierarchy) && !(currentStatsPopup.gameObject.activeInHierarchy && currentStatsPopup.mouseIsOn))
        { //if player left clicks & there is not a dragndrop & the shop is not open & theyre not clicking on popup UI
            ray = cam.ScreenPointToRay(Input.mousePosition); //ray from player POV
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, entityLayerMask)) //if it hits an entity
            {
                selectedEntity = hit.collider.GetComponent<Entity>();
                if (!selectedEntity) selectedEntity = hit.collider.GetComponentInParent<Entity>();
                if (selectedEntity) startPopup(selectedEntity); //double check to make sure the object has the entity script
                else closePopup();
            }
            else closePopup();

        }

        if (Input.GetMouseButtonDown(1)) closePopup(); //if player right clicks anywhere exit out of the selections

    }

    /// <summary>
    /// called when the remove button on the stats popup is called. deletes entity. has to be up here or the mouse click closes the popup first
    /// </summary>
    private void removeEntity()
    {
        if (!selectedEntity) return; //no selected entity
        selectedEntity.die();
        closePopup();
    }

    /// <summary>
    /// called when the move button on the stats popup is called. starts a drag n drop
    /// </summary>
    private void moveEntity()
    {

    }

}
