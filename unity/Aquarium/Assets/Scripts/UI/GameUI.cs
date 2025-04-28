using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    // private bool isOpen = false;
    private GameObject MenuItems;
    // private GameObject MenuButton;
    private Button ShopButton;
    private Button GalleryButton;
    private Button TankButton;
    private GameObject ShopUI;
    private GameObject GalleryUI;
    private GameObject TankUI;
    private GameObject menu;
    void Start()
    {
        // gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        ShopUI = GameObject.Find("ShopUI").gameObject;
        GalleryUI = GameObject.Find("GalleryUI").gameObject;
        TankUI = GameObject.Find("TankUI").gameObject;

        ShopUI.SetActive(false);
        GalleryUI.SetActive(false);
        TankUI.SetActive(false);

        menu = gameObject.transform.Find("Menu").gameObject;
        MenuItems = menu.transform.Find("MenuItems").gameObject;

        ShopButton = MenuItems.transform.Find("ShopButton").gameObject.GetComponent<Button>();
        GalleryButton = MenuItems.transform.Find("GalleryButton").gameObject.GetComponent<Button>();
        TankButton = MenuItems.transform.Find("TankButton").gameObject.GetComponent<Button>();

        ShopButton.onClick.AddListener(() => Open(ShopUI));
        GalleryButton.onClick.AddListener(() => Open(GalleryUI));
        TankButton.onClick.AddListener(() => Open(TankUI));


    }

    // Update is called once per frame
    void Update()
    {

    }
    void Open(GameObject ui)
    {
        ui.SetActive(true);
        gameObject.SetActive(false);
    }


}
