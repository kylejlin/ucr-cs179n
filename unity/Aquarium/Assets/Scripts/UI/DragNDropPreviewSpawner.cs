using UnityEngine;

public class DragNDropPreviewSpawner : MonoBehaviour
{
    public DragNDropPreview PPRay; //prefab
    private DragNDropPreview currentPPRay; //curently spawned preview
    void Start()
    {
       
    }

    /// <summary>
    /// start a dragNDrop session. Will show a preview of what the entity is and where it will spawn. restricts it to aquarium and doesnt allow collisions
    /// </summary>
    public void startPreview(Entity entity, Aquarium aquarium)
    {
        if (currentPPRay) Debug.LogWarning("Multiple DragNDrop previews present (there should only be one at any time)");
        currentPPRay = Instantiate(PPRay, new Vector3(0,0,0), Quaternion.identity, transform);
        currentPPRay.init(entity, aquarium, GetComponent<Camera>());
    }
}
