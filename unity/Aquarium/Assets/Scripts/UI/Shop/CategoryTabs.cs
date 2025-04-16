using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CategoryTabs : MonoBehaviour
{

    Button MobileCreatureTab;
    Button ImmobileCreatureTab;
    Button AllTab;
    Button DecorationTab;
    ShopManager shopManager;
    void Start()
    {
        // gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        shopManager = GetComponentInParent<ShopManager>();
        MobileCreatureTab = GameObject.Find("MobileCreatureTab").GetComponent<Button>();
        ImmobileCreatureTab = GameObject.Find("ImmobileCreatureTab").GetComponent<Button>();
        DecorationTab = GameObject.Find("DecorationTab").GetComponent<Button>();
        AllTab = GameObject.Find("AllTab").GetComponent<Button>();

        MobileCreatureTab.onClick.AddListener(() => SetCategory(typeof(MobileCreature)));
        ImmobileCreatureTab.onClick.AddListener(() => SetCategory(typeof(ImmobileCreature)));
        DecorationTab.onClick.AddListener(() => SetCategory(typeof(Decoration)));
        AllTab.onClick.AddListener(() => SetCategory());
    }
    void Update()
    {

    }
    public void SetCategory()
    {
        foreach (ShopItem item in shopManager.ShopItems)
        {
            item.gameObject.SetActive(true);
        }

    }
    public void SetCategory(System.Type category)
    {
        foreach (ShopItem item in shopManager.ShopItems)
        {
            if (item.GetComponent<ShopItem>().getEntity().GetType() == category)
            {
                item.gameObject.SetActive(true);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }

    }

}