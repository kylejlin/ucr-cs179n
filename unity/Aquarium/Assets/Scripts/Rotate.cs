using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 localRotationDirection;
    public float localRotationSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(localRotationDirection * localRotationSpeed * Time.deltaTime);
    }
}
