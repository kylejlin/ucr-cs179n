using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject trilobite;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(trilobite, new Vector3(0, 0, 0), Quaternion.identity); //spawns trilobite at origin and not rotated

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
