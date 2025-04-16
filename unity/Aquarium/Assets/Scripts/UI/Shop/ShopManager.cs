using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    private GameObject ShopGrid;
    private ShopItem ShopItem;
    private GameManager gameManager;
    private Entity SelectedEntity;
    private SelectedItem SelectedItemPreview;
    private CategoryTabs categoryTabs;
    public List<ShopItem> ShopItems = new List<ShopItem>();
    void Start()
    {
        gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        ShopGrid = gameObject.transform.Find("ShopGrid").gameObject;
        SelectedItemPreview = gameObject.transform.Find("SelectedItem").gameObject.GetComponent<SelectedItem>();
        ShopItem = Resources.Load<GameObject>("Shop/ShopItem").GetComponent<ShopItem>();
        SelectedEntity = gameManager.creatures[0];
        SelectedItemPreview.Setup(SelectedEntity);
        categoryTabs = gameObject.transform.Find("CategoryTabs").gameObject.GetComponent<CategoryTabs>();
        PopulateShop();
        categoryTabs.SetCategory(typeof(MobileCreature));
    }

    // Update is called once per frame
    void Update()
    {

    }
    void PopulateShop()
    {
        foreach (Entity item in gameManager.creatures)
        {
            ShopItem shopButton = Instantiate(ShopItem.gameObject, ShopGrid.transform).GetComponent<ShopItem>();
            shopButton.Setup(item);
            ShopItems.Add(shopButton);
            print(shopButton.getButton());
            shopButton.getButton().onClick.AddListener(() => select(item));

        }
    }
    public void CloseShop()
    {
        gameObject.SetActive(false);
    }
    public void select(Entity entity)
    {
        SelectedEntity = entity;
        SelectedItemPreview.ChangeSelected(SelectedEntity);

    }
}
