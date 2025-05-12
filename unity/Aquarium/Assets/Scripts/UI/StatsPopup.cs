using UnityEngine;
using TMPro;

public class StatsPopup : MonoBehaviour
{
    public TMP_Text statsText; //set in editor
    public RectTransform box; //set in editor
    private Entity selectedEntity; //current entity being tracked

    private int textVerticalPadding;

    void Start()
    {
        
    }

    void Update()
    {
        if(selectedEntity) {if(gameObject.activeInHierarchy) setText(selectedEntity.getCurrStats());} //update text every frame
        else closePopup(); //if entity died or dissappeared or w/e
    }

/// <summary> start popup or reset it for a new entity </summary>
/// <param name="entity"> reference to entity in scene, to highlight and display its stats </param>
    public void startPopup(Entity entity){ 
        setSelectedEntity(entity);
        if(entity) {
            gameObject.SetActive(true);
        }
        else Debug.Log("Null entity in Stats popup");
    }

/// <summary> hide stats and remove entity outline </summary>
    public void closePopup(){ //hide 
        setSelectedEntity(null);
        setText("No entity selected");
        gameObject.SetActive(false);
    }

    private void setSelectedEntity(Entity entity){ 
        if(selectedEntity) selectedEntity.setOutline(false); //switch which entity is outlined. there should never be more than one
        selectedEntity = entity;
        if(entity) {
            setText(entity.getCurrStats());
            entity.setOutline(true);
            print("set "+entity+"outline true");
        }
    }

    private void setText(string newText){ //set the text on the popup and adjust box size
        statsText.text = newText;
        box.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, statsText.preferredHeight + 50); //idk how this anchor stuff works. this is probably not the best way
    }
}
