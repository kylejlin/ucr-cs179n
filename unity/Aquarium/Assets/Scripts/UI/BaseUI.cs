using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaseUI : MonoBehaviour
{
    protected GameManager gameManager;
    private Button CloseButton;
    private GameObject GameUI;
    protected GameObject toast;
    protected void Start()
    {
        gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        GameUI = transform.parent.Find("GameUI").gameObject;
        toast = gameObject.transform.Find("Toast").gameObject;
        CloseButton = gameObject.transform.Find("CloseButton").gameObject.GetComponent<Button>();


        toast.SetActive(false);

        CloseButton.onClick.AddListener(() => CloseShop());
    }


    public void CloseShop()
    {
        toast.SetActive(false);
        gameObject.SetActive(false);
        GameUI.SetActive(true);

    }

    public void ShowToast(string message, float duration = 4f)
    {
        StartCoroutine(ShowToastCoroutine(message, duration));
    }

    private IEnumerator ShowToastCoroutine(string message, float duration)
    {
        toast.GetComponentInChildren<TextMeshProUGUI>().text = message;
        toast.SetActive(true);
        yield return new WaitForSeconds(duration);
        toast.SetActive(false);
    }
}
