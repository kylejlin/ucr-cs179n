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
    ShopUI shopManager;
    void Start()
    {
        shopManager = GetComponentInParent<ShopUI>();
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
            if (category.IsAssignableFrom(item.GetComponent<ShopItem>().getEntity().GetType()))
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