using UnityEngine;
using UnityEngine.UI;

public class Preview : MonoBehaviour
{
    private GameObject modelPrefab;
    private GameObject modelInstance; // Instance of the 3D model
    private GameObject modelCamera; // Camera that renders the 3D model
    private RawImage rawImage;
    private Entity entity;
    void Start()
    {
    }
    void Update()
    {
        if (modelInstance != null)
        {
            modelInstance.transform.Rotate(Vector3.up, 40f * Time.deltaTime); // 40 degrees per second
        }
    }
    public void ChangeSelected(Entity selectedEntity)
    {
        this.entity = selectedEntity;
        if (modelInstance != null)
        {
            Destroy(modelInstance);
        }
        rawImage = gameObject.GetComponent<RawImage>();
        rawImage.SetNativeSize();
        this.modelPrefab = entity.gameObject;
        modelInstance = Instantiate(modelPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
        if(modelInstance.TryGetComponent<Entity>(out Entity entityInstance)) {entityInstance.initDisplayMode(); } //display mode turns off update() and sets creature to adult size
        modelInstance.layer = LayerMask.NameToLayer("ShopSelectedItem");
        SetLayerRecursively(modelInstance.transform, LayerMask.NameToLayer("ShopSelectedItem"));

    }

    public void Setup(Entity selectedEntity)
    {
        this.entity = selectedEntity;
        rawImage = gameObject.AddComponent<RawImage>();
        rawImage.SetNativeSize();

        this.modelPrefab = entity.gameObject;
        modelInstance = Instantiate(modelPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
        if(modelInstance.TryGetComponent<Entity>(out Entity entityInstance)) {entityInstance.initShopMode(); } //display mode turns off update() and sets creature to adult size
        modelInstance.layer = LayerMask.NameToLayer("ShopSelectedItem");
        SetLayerRecursively(modelInstance.transform, LayerMask.NameToLayer("ShopSelectedItem"));
        modelCamera = new GameObject(entity.name + "Camera");
        modelCamera.transform.SetParent(transform);

        Camera cam = modelCamera.AddComponent<Camera>();


        // Set position and rotation (optional)
        modelCamera.transform.position = new Vector3(0, 3.6f, -5f);
        modelCamera.transform.rotation = Quaternion.Euler(50, 0, 0);

        // Customize the camera (optional)
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);
        cam.orthographic = false;
        cam.cullingMask = LayerMask.GetMask("ShopSelectedItem");

        RenderTexture rt = new RenderTexture(512, 512, 16);
        cam.targetTexture = rt;
        rawImage.texture = rt;
    }
    public Entity getEntity()
    {
        return entity;
    }
    void SetLayerRecursively(Transform obj, int newLayer)
    {
        obj.gameObject.layer = newLayer;
        foreach (Transform child in obj)
        {
            SetLayerRecursively(child, newLayer);
        }
    }

}