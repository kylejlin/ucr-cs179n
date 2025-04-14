using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private Transform previewGrid; // Reference to the Preview Grid in the UI
    private GameObject previewPrefab; // Prefab for the preview items
    private RawImage SelectedItem; // The RawImage to display the big model
    private GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        previewGrid = gameObject.transform.Find("PreviewGrid");
        previewPrefab = Resources.Load<GameObject>("Shop/ShopItem");
        SelectedItem = gameObject.transform.Find("SelectedItem").gameObject.GetComponent<RawImage>();

        PopulateShop();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void PopulateShop()
    {

        Debug.Log("PopulateShop");
        foreach (GameObject item in gameManager.trilobites)
        {
            Debug.Log(item.name);
            // Create a button for each preview item in the grid
            GameObject previewButton = Instantiate(previewPrefab, previewGrid);
            previewButton.GetComponent<Image>().sprite = item.GetComponentInChildren<SpriteRenderer>().sprite;
            Button button = previewButton.GetComponent<Button>();

            // Set the action for the button (show the large model)
            button.onClick.AddListener(() => ShowBigModel(item));
        }
    }
    void ShowBigModel(GameObject item)
    {
        // Set the large 3D model's RenderTexture to the RawImage
        RenderTexture renderTexture = new RenderTexture(1024, 1024, 16);
        renderTexture.Create();
        SelectedItem.texture = renderTexture;

        // Instantiate the model in the scene for the camera to render
        GameObject modelInstance = Instantiate(item, Vector3.zero, Quaternion.identity);
        modelInstance.SetActive(true); // Make sure the model is active
    }
}
