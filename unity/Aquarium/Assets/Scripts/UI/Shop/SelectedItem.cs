using UnityEngine;
using UnityEngine.UI;

public class SelectedItem : Preview
{
    Button buyButton;
    GameManager gameManager;
    ShopUI shopManager;
    TMPro.TextMeshProUGUI text;


    void Start()
    {
        gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        shopManager = GetComponentInParent<ShopUI>();
        buyButton = gameObject.transform.Find("BuyButton").gameObject.GetComponent<Button>();
        buyButton.onClick.AddListener(() => BuyItem());
        text = gameObject.transform.Find("Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>();

    }

    void BuyItem()
    {
        if (gameManager.CanBuy(this.getEntity().getBuyMoney()))
        {
            gameManager.buy(this.getEntity().getBuyMoney());
            gameManager.addEntity(this.getEntity());
            shopManager.CloseShop();
        }
        else
        {
            shopManager.ShowToast("Not enough coins", 1);
        }
    }
    public new void Setup(Entity selectedEntity)
    {
        base.Setup(selectedEntity);
        text.text = selectedEntity.getShopStats();

    }
    public new void ChangeSelected(Entity selectedEntity)
    {
        base.ChangeSelected(selectedEntity);
        text.text = selectedEntity.getShopStats();

    }
}