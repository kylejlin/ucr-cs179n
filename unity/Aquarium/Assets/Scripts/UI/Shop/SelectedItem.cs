using UnityEngine;
using UnityEngine.UI;

public class SelectedItem : Preview
{
    void Start()
    {
        ShopManager parent = GetComponentInParent<ShopManager>();

        Button button = gameObject.GetComponent<Button>();
        AddTrilobite1.onClick.AddListener(() => AddTrilobite(0));
    }
    void Update()
    {

    }

    void selectItem()
    {

    }

}