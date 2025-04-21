using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;

public class ShopUI : MonoBehaviour
{
    private GameObject ShopGrid;
    private ShopItem ShopItem;
    private GameManager gameManager;
    private Entity SelectedEntity;
    private Preview SelectedItemPreview;
    private CategoryTabs categoryTabs;
    private Button CloseButton;
    private GameObject GameUI;
    public List<ShopItem> ShopItems = new List<ShopItem>();
    private GameObject toast;
    public bool galleryMode = false;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        GameUI = transform.parent.Find("GameUI").gameObject;
        ShopGrid = gameObject.transform.Find("ShopGrid").gameObject;
        toast = gameObject.transform.Find("Toast").gameObject;
        SelectedItemPreview = gameObject.transform.Find("SelectedItem").gameObject.GetComponent<Preview>();
        categoryTabs = gameObject.transform.Find("CategoryTabs").gameObject.GetComponent<CategoryTabs>();
        CloseButton = gameObject.transform.Find("CloseButton").gameObject.GetComponent<Button>();

        ShopItem = Resources.Load<GameObject>("Shop/ShopItem").GetComponent<ShopItem>();

        toast.SetActive(false);

        SelectedEntity = gameManager.creatures[0];
        SelectedItemPreview.Setup(SelectedEntity);
        if (galleryMode)
            SelectedItemPreview.setCollected(gameManager.isCollected(SelectedEntity));

        PopulateShop();
        categoryTabs.SetCategory();
        CloseButton.onClick.AddListener(() => CloseShop());
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
    public void CloseShop()
    {
        gameObject.SetActive(false);
        GameUI.SetActive(true);

    }
    private void select(Entity entity)
    {
        SelectedEntity = entity;
        SelectedItemPreview.ChangeSelected(SelectedEntity);
        if (galleryMode)
            SelectedItemPreview.setCollected(gameManager.isCollected(SelectedEntity));

    }

    public void ShowToast(string message, float duration = 2f)
    {
        StartCoroutine(ShowToastCoroutine(message, duration));
    }

    private IEnumerator ShowToastCoroutine(string message, float duration)
    {
        toast.GetComponentInChildren<TextMeshProUGUI>().text = message;
        toast.SetActive(true);
        yield return new WaitForSeconds(duration);
        toast.SetActive(false);
    }
}
