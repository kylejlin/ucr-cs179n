using UnityEngine;

public class TankItem : Preview
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setText(string text)
    {
        gameObject.transform.Find("Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = text;
    }

}
