using UnityEngine;

public class Menu : MonoBehaviour
{
    private bool isOpen = false;
    private GameObject MenuButton;
    private GameObject ShopButton;
    private GameObject GalleryButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
