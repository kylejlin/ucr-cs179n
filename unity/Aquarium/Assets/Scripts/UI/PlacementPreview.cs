using UnityEngine;
using System.Collections;

public class PlacementPreview : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // public Camera camera;

    void Start(){
        print(camera.transform.localPosition);

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;
            
            // Do something with the object that was hit by the raycast.
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
