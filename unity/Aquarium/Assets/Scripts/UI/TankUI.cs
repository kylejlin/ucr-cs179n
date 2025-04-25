using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TankUI : BaseUI
{
    private Button AddTankButton;
    private Button SelectTankButton;
    private Button NextTankButton;
    private Button PrevTankButton;

    protected new void Start()
    {
        base.Start();
        AddTankButton = gameObject.transform.Find("AddTankButton").gameObject.GetComponent<Button>();
        SelectTankButton = gameObject.transform.Find("SelectTankButton").gameObject.GetComponent<Button>();
        NextTankButton = gameObject.transform.Find("NextTankButton").gameObject.GetComponent<Button>();
        PrevTankButton = gameObject.transform.Find("PrevTankButton").gameObject.GetComponent<Button>();
        AddTankButton.onClick.AddListener(() => AddTank());

        SelectTankButton.onClick.AddListener(() => SelectTank());
        NextTankButton.onClick.AddListener(() => NextTank());
        PrevTankButton.onClick.AddListener(() => PrevTank());
    }
    void AddTank()
    {
        gameManager.addTank();

    }
    void SelectTank()
    {
        gameManager.selectTank(0);
    }
    void NextTank()
    {
        gameManager.nextTank();
    }
    void PrevTank()
    {
        gameManager.PrevTank();

    }
}
