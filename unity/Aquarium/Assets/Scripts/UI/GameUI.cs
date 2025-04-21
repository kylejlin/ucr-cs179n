using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    // private bool isOpen = false;
    private GameObject MenuButton;
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
        ShopUI = GameObject.Find("ShopUI").gameObject;
        ShopUI.SetActive(false);
        menu = gameObject.transform.Find("Menu").gameObject;
        MenuButton = menu.transform.Find("MenuButton").gameObject;
        ShopButton = MenuButton.transform.Find("ShopButton").gameObject.GetComponent<Button>();
        // GalleryButton = MenuButton.transform.Find("GalleryButton").gameObject.GetComponent<Button>();

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
