using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Coroutine moveCoroutine;
    private Transform target;
    public float rotationSpeed = 50f; // Speed of rotation
    public float zoomSpeed = 1f; // Speed of zoom in/out
    public float minFov = 20f; // Minimum field of view
    public float maxFov = 60f; // Maximum field of view
    public float zoomSmoothTime = 0.3f; // Time to smooth the zooming effect
    public float movementSpeed = 5f; // Speed of camera movement
    public float distance = 150f; // Distance from the target

    private Camera cam;
    private float targetFov;
    private float currentFovVelocity;
    void Start()
    {
        cam = GetComponent<Camera>();
        targetFov = cam.fieldOfView;
    }

    void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A))
            horizontal = -1f;
        else if (Input.GetKey(KeyCode.D))
            horizontal = 1f;

        if (Input.GetKey(KeyCode.W))
            vertical = 1f;
        else if (Input.GetKey(KeyCode.S))
            vertical = -1f;

        if (horizontal != 0 || vertical != 0)
        {
            transform.RotateAround(target.position, Vector3.up, horizontal * rotationSpeed * movementSpeed * Time.deltaTime);
            transform.RotateAround(target.position, transform.right, vertical * rotationSpeed * movementSpeed * Time.deltaTime);
            if (transform.up.y < 0)
            {
                transform.RotateAround(target.position, transform.right, -vertical * rotationSpeed * Time.deltaTime * movementSpeed);
            }
        }

        if (Input.GetKey(KeyCode.UpArrow))
            targetFov -= zoomSpeed;
        if (Input.GetKey(KeyCode.DownArrow))
            targetFov += zoomSpeed;

        targetFov = Mathf.Clamp(targetFov, minFov, maxFov);
        cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetFov, ref currentFovVelocity, zoomSmoothTime);

    }
    
    public void setTarget(Transform newTarget)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        target = newTarget;
        moveCoroutine = StartCoroutine(SmoothMoveAndRotate());
    }

    private IEnumerator SmoothMoveAndRotate()
    {

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + distance, target.position.z);
        Quaternion targetRotation = Quaternion.Euler(90, 0, 0);

        // Smoothly move to the target position
        while (Vector3.Distance(transform.position, targetPosition) > 1 || Quaternion.Angle(transform.rotation, targetRotation) > 10)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, movementSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        transform.rotation = targetRotation;

    }
}
