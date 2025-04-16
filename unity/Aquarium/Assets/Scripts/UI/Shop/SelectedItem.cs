using UnityEngine;
using UnityEngine.UI;

public class SelectedItem : Preview
{
    Button buyButton;
    GameManager gameManager;
    ShopManager shopManager;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        shopManager = GetComponentInParent<ShopManager>();
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