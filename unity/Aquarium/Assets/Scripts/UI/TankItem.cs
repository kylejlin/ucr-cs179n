using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TankItem : Preview
{
    public void setText(string text)
    {
        gameObject.transform.Find("Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = text;
    }

    public void Setup(Entity item, UnityAction onClick)
    {
        base.Setup(item);
        gameObject.GetComponent<Button>().onClick.AddListener(onClick);

    }
}

