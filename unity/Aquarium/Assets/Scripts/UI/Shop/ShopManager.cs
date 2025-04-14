using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private Transform previewGrid; // Reference to the Preview Grid in the UI
    private GameObject previewPrefab; // Prefab for the preview items
    private GameManager gameManager;
    private Entity SelectedEntity;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        previewGrid = gameObject.transform.Find("PreviewGrid");
        previewPrefab = Resources.Load<GameObject>("Shop/ShopItem");

        PopulateShop();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void PopulateShop()
    {

        Debug.Log("PopulateShop");
        foreach (GameObject item in gameManager.creatures)
        {
            // Debug.Log(item.name);
            // Create a button for each preview item in the grid
            GameObject previewButton = Instantiate(previewPrefab, previewGrid);
            // previewButton.GetComponent<Image>().sprite = item.GetComponentInChildren<SpriteRenderer>().sprite;
            // Button button = previewButton.GetComponent<Button>();

            // // Set the action for the button (show the large model)
            // button.onClick.AddListener(() => ShowBigModel(item));
        }
    }

}
