using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ShopUI : MonoBehaviour
{
    private GameObject ShopGrid;
    private ShopItem ShopItem;
    private GameManager gameManager;
    private Entity SelectedEntity;
    private SelectedItem SelectedItemPreview;
    private CategoryTabs categoryTabs;
    private Button CloseButton;
    private GameObject GameUI;
    public List<ShopItem> ShopItems = new List<ShopItem>();

    void Start()
    {
        gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        GameUI = GameObject.Find("GameUI").gameObject;
        ShopGrid = gameObject.transform.Find("ShopGrid").gameObject;
        SelectedItemPreview = gameObject.transform.Find("SelectedItem").gameObject.GetComponent<SelectedItem>();
        categoryTabs = gameObject.transform.Find("CategoryTabs").gameObject.GetComponent<CategoryTabs>();
        CloseButton = gameObject.transform.Find("CloseButton").gameObject.GetComponent<Button>();

        ShopItem = Resources.Load<GameObject>("Shop/ShopItem").GetComponent<ShopItem>();

        SelectedEntity = gameManager.creatures[0];
        SelectedItemPreview.Setup(SelectedEntity);

        PopulateShop();
        categoryTabs.SetCategory();
        CloseButton.onClick.AddListener(() => CloseShop());

    }

    void PopulateShop()
    {
        foreach (Entity item in gameManager.creatures)
        {
            ShopItem shopButton = Instantiate(ShopItem.gameObject, ShopGrid.transform).GetComponent<ShopItem>();
            shopButton.Setup(item);
            ShopItems.Add(shopButton);
            shopButton.getButton().onClick.AddListener(() => select(item));

        }
    }
    public void CloseShop()
    {
        gameObject.SetActive(false);
        GameUI.SetActive(true);

    }
    private void select(Entity entity)
    {
        SelectedEntity = entity;
        SelectedItemPreview.ChangeSelected(SelectedEntity);

    }
}
