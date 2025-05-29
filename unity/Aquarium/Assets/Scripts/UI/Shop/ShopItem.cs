using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : Preview
{
    Button button;
    TMPro.TextMeshProUGUI nameText;
    void Start()
    {
        nameText = gameObject.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>();
    }
    void Update()
    {

    }
    public new void Setup(Entity selectedEntity)
    {
        base.Setup(selectedEntity);
        nameText.text = selectedEntity.name;

    }
    public Button getButton()
    {
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
        return button;
    }

}