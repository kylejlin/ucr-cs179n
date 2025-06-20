using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TankUI : BaseUI
{
    private Button AddTankButton;
    private Button NextTankButton;
    private Button PrevTankButton;
    [SerializeField]
    private GameObject EntitiesGrid;
    private TankItem TankItem;
    private List<TankItem> TankItems = new List<TankItem>();
    private bool hasStarted = false;
    [SerializeField] private float costOfTank = 100f;
    MouseUIManager mouseUIManager;
    protected new void Start()
    {
        base.Start();
        // EntitiesGrid = gameObject.transform.Find("EntitiesGrid").gameObject;
        AddTankButton = gameObject.transform.Find("AddTankButton").gameObject.GetComponent<Button>();
        NextTankButton = gameObject.transform.Find("NextTankButton").gameObject.GetComponent<Button>();
        PrevTankButton = gameObject.transform.Find("PrevTankButton").gameObject.GetComponent<Button>();
        TankItem = Resources.Load<GameObject>("Shop/TankItem").GetComponent<TankItem>();
        mouseUIManager = GameObject.Find("Main Camera").GetComponent<MouseUIManager>();

        AddTankButton.onClick.AddListener(() => AddTank());

        NextTankButton.onClick.AddListener(() => NextTank());
        PrevTankButton.onClick.AddListener(() => PrevTank());
        PopulateTank(); hasStarted = true;
    }
    void AddTank()
    {
        if (gameManager.CanBuy(costOfTank))
        {
            gameManager.buy(costOfTank);
            gameManager.addTank();
        }
        else
        {
            ShowToast("Not enough money!", 4);
        }
    }

    void NextTank()
    {
        gameManager.nextTank();
        PopulateTank();
    }
    void PrevTank()
    {
        gameManager.PrevTank();
        PopulateTank();

    }
    void OnEnable()
    {
        if (!hasStarted)
            return;
        PopulateTank();
    }
    void PopulateTank()
    {
        // Clear previous items
        foreach (TankItem child in TankItems)
        {
            Destroy(child.gameObject);
        }
        TankItems.Clear();
        foreach (Entity item in gameManager.getTank().entities)
        {
            TankItem tankItem = Instantiate(TankItem.gameObject, EntitiesGrid.transform).GetComponent<TankItem>();
            tankItem.Setup(item, () => { CloseShop(); mouseUIManager.startPopup(item); });
            tankItem.setText(item.name);
            TankItems.Add(tankItem);

        }
    }
}
