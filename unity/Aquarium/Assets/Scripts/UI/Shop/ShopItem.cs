using UnityEngine;
using UnityEngine.UI;

public class ShopItem : Preview
{
    Button button;
    void Start()
    {
    }
    void Update()
    {

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