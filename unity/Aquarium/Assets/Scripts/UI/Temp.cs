using UnityEngine;
using UnityEngine.UI;

public class Temp : MonoBehaviour
{
    private GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();

        Button AddTrilobite1 = GameObject.Find("Add Trilobite1").GetComponent<Button>();
        Button AddTrilobite2 = GameObject.Find("Add Trilobite2").GetComponent<Button>();

        AddTrilobite1.onClick.AddListener(() => AddTrilobite(0));
        AddTrilobite2.onClick.AddListener(() => AddTrilobite(1));
    }

    void AddTrilobite(int index)
    {
        gameManager.addEntity(gameManager.creatures[index], gameManager.getTank());
    }

    void Update()
    {

    }
}
