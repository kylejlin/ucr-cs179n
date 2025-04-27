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
        if (modelInstance.TryGetComponent<Entity>(out Entity entityInstance)) { entityInstance.initShopMode(); } //display mode turns off update() and sets creature to adult size
        modelInstance.layer = LayerMask.NameToLayer("ShopSelectedItem");
        SetLayerRecursively(modelInstance.transform, LayerMask.NameToLayer("ShopSelectedItem"));
        Rigidbody rb = modelInstance.GetComponent<Rigidbody>(); //initShopMode() does this now as well
        if (rb != null)
        {
            Destroy(rb);
        }

        FrameObject(modelInstance, modelCamera.GetComponent<Camera>());

    }

    public void Setup(Entity selectedEntity)
    {
        rawImage = gameObject.AddComponent<RawImage>();
        rawImage.SetNativeSize();
        Camera cam;
        if (!modelCamera)
        {
            modelCamera = new GameObject(selectedEntity.name + "Camera");
            cam = modelCamera.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0, 0, 0, 0);
            cam.orthographic = false;
            cam.cullingMask = LayerMask.GetMask("ShopSelectedItem");
            cam.orthographic = true;

        }
        else
        {
            cam = modelCamera.GetComponent<Camera>();
        }
        modelCamera.transform.SetParent(transform);
        modelCamera.transform.position = new Vector3(0, 3.6f, -5f);
        modelCamera.transform.rotation = Quaternion.Euler(50, 0, 0);


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

    public void FrameObject(GameObject targetObject, Camera camera)
    {

        Bounds bounds = CalculateBounds(targetObject);

        float objectSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float distance = CalculateRequiredDistance(camera, bounds, objectSize);

        Vector3 direction = camera.transform.rotation * Vector3.forward;
        camera.transform.position = bounds.center - direction * distance;
    }

    private Bounds CalculateBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(obj.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    private float CalculateRequiredDistance(Camera camera, Bounds bounds, float objectSize)
    {
        if (camera.orthographic)
        {
            camera.orthographicSize = objectSize * 0.5f;
            return objectSize; // distance doesn't matter for ortho, only size
        }
        else
        {
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView);
            float distance = (objectSize * 0.5f) / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            distance *= 1.1f;

            return distance;
        }
    }


    public void setCollected(bool collected = true)
    {
        if (collected)
        { rawImage.color = new Color(255, 255, 255, 1); }
        else
        { rawImage.color = new Color(0, 0, 0, 0); }
    }
}