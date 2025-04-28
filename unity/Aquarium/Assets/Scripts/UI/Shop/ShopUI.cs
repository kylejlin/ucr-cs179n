using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShopUI : BaseUI
{
    private GameObject ShopGrid;
    private ShopItem ShopItem;
    private Entity SelectedEntity;
    private Preview SelectedItemPreview;
    private CategoryTabs categoryTabs;

    public List<ShopItem> ShopItems = new List<ShopItem>();
    public bool galleryMode = false;
    protected new void Start()
    {
        base.Start();
        ShopGrid = gameObject.transform.Find("ShopGrid").gameObject;
        SelectedItemPreview = gameObject.transform.Find("SelectedItem").gameObject.GetComponent<Preview>();
        categoryTabs = gameObject.transform.Find("CategoryTabs").gameObject.GetComponent<CategoryTabs>();

        ShopItem = Resources.Load<GameObject>("Shop/ShopItem").GetComponent<ShopItem>();


        SelectedEntity = gameManager.creatures[0];
        SelectedItemPreview.Setup(SelectedEntity);
        if (galleryMode)
            SelectedItemPreview.setCollected(gameManager.isCollected(SelectedEntity));

        PopulateShop();
        categoryTabs.SetCategory();
        OnEnable();
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
    void OnEnable()
    {
        if (galleryMode)
        {
            foreach (ShopItem item in ShopItems)
            {

                if (gameManager.isCollected(item.getEntity()))
                {
                    item.setCollected(true);
                }
                else
                {
                    item.setCollected(false);
                }
            }
        }
    }

    private void select(Entity entity)
    {
        SelectedEntity = entity;
        SelectedItemPreview.ChangeSelected(SelectedEntity);
        if (galleryMode)
            SelectedItemPreview.setCollected(gameManager.isCollected(SelectedEntity));

    }


}
