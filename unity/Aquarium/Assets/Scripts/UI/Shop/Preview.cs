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
        if(modelInstance.TryGetComponent<Entity>(out Entity entityInstance)) {entityInstance.initShopMode(); } //display mode turns off update() and sets creature to adult size
        modelInstance.layer = LayerMask.NameToLayer("ShopSelectedItem");
        SetLayerRecursively(modelInstance.transform, LayerMask.NameToLayer("ShopSelectedItem"));
        Rigidbody rb = modelInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb);
        }

    }

    public void Setup(Entity selectedEntity)
    {
        rawImage = gameObject.AddComponent<RawImage>();
        rawImage.SetNativeSize();

        modelCamera = new GameObject(selectedEntity.name + "Camera");
        modelCamera.transform.SetParent(transform);
        Camera cam = modelCamera.AddComponent<Camera>();
        modelCamera.transform.position = new Vector3(0, 3.6f, -5f);
        modelCamera.transform.rotation = Quaternion.Euler(50, 0, 0);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);
        cam.orthographic = false;
        cam.cullingMask = LayerMask.GetMask("ShopSelectedItem");
        RenderTexture rt = new RenderTexture(512, 512, 16);
        cam.targetTexture = rt;
        rawImage.texture = rt;


        ChangeSelected(selectedEntity);

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