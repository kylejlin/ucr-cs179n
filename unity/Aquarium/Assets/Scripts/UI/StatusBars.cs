using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameObject : MonoBehaviour
{
    private Slider happiness; // Assign in Inspector
    private TextMeshProUGUI happinessText;

    private GameObject money;
    private TextMeshProUGUI moneyText;

    private Slider consume;
    private TextMeshProUGUI hunger;
    private TextMeshProUGUI supply;

    private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();

        happiness = gameObject.transform.Find("happiness").gameObject.GetComponent<Slider>();
        happiness.interactable = false;
        happinessText = happiness.GetComponentInChildren<TextMeshProUGUI>();

        money = gameObject.transform.Find("money").gameObject;
        moneyText = money.GetComponentInChildren<TextMeshProUGUI>();

        if(gameObject.transform.Find("consume")) {
            consume = gameObject.transform.Find("consume").gameObject.GetComponent<Slider>();
            consume.interactable = false;
            hunger = consume.transform.Find("hunger").gameObject.GetComponent<TextMeshProUGUI>();
            supply = consume.transform.Find("supply").gameObject.GetComponent<TextMeshProUGUI>();}
    }

    // Update is called once per frame
    void Update()
    {
        happiness.value = gameManager.getHappiness();
        happinessText.text = Mathf.RoundToInt(happiness.value * 100) + "%";

        moneyText.text = "Money: " + gameManager.getMoney().ToString();

        int hungerValue = gameManager.getHunger();
        int supplyValue = gameManager.getAlgaesHealth();
        if(consume)consume.value = (float)supplyValue / (hungerValue + supplyValue);
        if(hunger) hunger.text = "Hunger: " + hungerValue.ToString();
        if(supply) supply.text = "Supply: " + supplyValue.ToString();
    }
}
