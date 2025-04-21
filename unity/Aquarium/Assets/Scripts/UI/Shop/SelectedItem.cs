using UnityEngine;
using UnityEngine.UI;

public class SelectedItem : Preview
{
    Button buyButton;
    GameManager gameManager;
    ShopUI shopManager;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        shopManager = GetComponentInParent<ShopUI>();
        buyButton = gameObject.transform.Find("BuyButton").gameObject.GetComponent<Button>();
        buyButton.onClick.AddListener(() => BuyItem());
    }
    void Update()
    {

    }
    void BuyItem()
    {
        if (gameManager.CanBuy(this.getEntity().getBuyMoney()))
        {
            gameManager.buy(this.getEntity().getBuyMoney());
            gameManager.addEntity(this.getEntity(), gameManager.aquarium);
            shopManager.CloseShop();
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }

}