using UnityEngine;
using UnityEngine.UI;

public class SelectedItem : MonoBehaviour
{
    public GameObject modelPrefab; // todo: just a test
    private GameObject modelInstance; // Instance of the 3D model
    private GameObject modelCamera; // Camera that renders the 3D model
    private RawImage rawImage;
    void Start()
    {
        rawImage = gameObject.GetComponent<RawImage>();
        modelCamera = new GameObject("SelectedItemCamera");

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
        modelInstance = Instantiate(modelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        modelInstance.layer = LayerMask.NameToLayer("ShopSelectedItem");
        SetLayerRecursively(modelInstance.transform, LayerMask.NameToLayer("ShopSelectedItem"));

    }
    void Update()
    {
        if (modelInstance != null)
        {
            modelInstance.transform.Rotate(Vector3.up, 40f * Time.deltaTime); // 40 degrees per second
        }
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