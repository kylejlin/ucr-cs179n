using UnityEngine;
using UnityEngine.UI;

public class SelectedItem : MonoBehaviour
{
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
        cam.backgroundColor = Color.black;
        cam.orthographic = false;

        RenderTexture rt = new RenderTexture(512, 512, 16);
        cam.targetTexture = rt;
        rawImage.texture = rt;
        // modelInstance = Instantiate(modelPrefab, new Vector3(0, 2.8f, 0), Quaternion.identity);

    }
}