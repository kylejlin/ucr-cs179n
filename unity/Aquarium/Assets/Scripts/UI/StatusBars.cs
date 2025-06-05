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
        float h = gameManager.getHappinessRatio();
        happinessText.text = "Happiness: " + Mathf.RoundToInt(h*100) +"%"; //should it be by aquarium? I feel like a total value is not as helpful. show bars above each aquarium?
        happiness.value = h;

        moneyText.text = gameManager.getMoney().ToString("F2");

        int hungerValue = gameManager.getHunger();
        int supplyValue = gameManager.getAlgaesHealth();
        if(consume)consume.value = (float)supplyValue / (hungerValue + supplyValue);
        if(hunger) hunger.text = "Hunger: " + hungerValue.ToString();
        if(supply) supply.text = "Supply: " + supplyValue.ToString();
    }
}
