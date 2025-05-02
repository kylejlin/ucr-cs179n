using UnityEngine;
using TMPro;

public class StatsPopup : MonoBehaviour
{
    public int width = 300;
    private TMP_Text statsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statsText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setText(string newText){
        statsText.text = newText;
    }
}
