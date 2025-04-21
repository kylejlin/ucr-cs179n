using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField]
    Vector2 spacing;
    Vector2 menuPosition;
    Button menuButton;
    MenuItem[] menuItems;
    bool isOpen = false;
    int menuItemsCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuItemsCount = transform.childCount - 1;
        menuItems = new MenuItem[menuItemsCount];
        for (int i = 0; i < menuItemsCount; i++)
        {
            menuItems[i] = transform.GetChild(i + 1).GetComponent<MenuItem>();
        }
        menuButton = gameObject.GetComponent<Button>();
        menuButton.transform.SetAsLastSibling();
        menuButton.onClick.AddListener(ToggleMenu);

        menuPosition = menuButton.transform.position;


    }
    void ResetPositions()
    {
        for (int i = 0; i < menuItemsCount; i++)
        {
            menuItems[i].trans.position = menuPosition;
        }
    }
    void ToggleMenu()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            for (int i = 0; i < menuItemsCount; i++)
            {
                menuItems[i].trans.position = menuPosition + spacing * (i + 1);
            }
        }
        else
        {
            ResetPositions();
        }
    }
}
