using UnityEngine;
using TMPro;

public class StatsPopup : MonoBehaviour
{
    public TMP_Text statsText; //set in editor
    public RectTransform box; //set in editor

    private int textVerticalPadding;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setText(string newText){
        statsText.text = newText;
        box.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, statsText.preferredHeight + 50); //idk how this anchor stuff works. this is probably not the best way
        // statsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, statsText.preferredHeight/2 + 25);
    }
}
