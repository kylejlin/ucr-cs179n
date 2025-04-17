using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    // private bool isOpen = false;
    private GameObject MenuItems;
    // private GameObject MenuButton;
    private Button ShopButton;
    // private Button GalleryButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // private GameManager gameManager;
    private GameObject ShopUI;
    private GameObject menu;
    void Start()
    {
        // gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        ShopUI = GameObject.Find("ShopUI");
        menu = gameObject.transform.Find("Menu").gameObject;
        MenuItems = menu.transform.Find("MenuItems").gameObject;
        ShopButton = MenuItems.transform.Find("ShopButton").gameObject.GetComponent<Button>();
        // GalleryButton = MenuItems.transform.Find("GalleryButton").gameObject.GetComponent<Button>();

        ShopButton.onClick.AddListener(() => OpenShop());

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OpenShop()
    {
        ShopUI.SetActive(true);
        gameObject.SetActive(false);
    }

}
