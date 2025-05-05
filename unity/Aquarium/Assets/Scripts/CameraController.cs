using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    public float rotationSpeed = 50f; // Speed of rotation
    public float zoomSpeed = 1f; // Speed of zoom in/out
    public float minFov = 20f; // Minimum field of view
    public float maxFov = 60f; // Maximum field of view
    public float zoomSmoothTime = 0.3f; // Time to smooth the zooming effect
    public float movementSpeed = 5f; // Speed of camera movement
    public float distance = 150f; // Distance from the target
    private bool isMoving = false; // Flag to check if the camera is already moving

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
            transform.RotateAround(target.position, Vector3.up, horizontal * rotationSpeed * Time.deltaTime);
            transform.RotateAround(target.position, transform.right, vertical * rotationSpeed * Time.deltaTime);
            if (transform.up.y < 0)
            {
                transform.RotateAround(target.position, transform.right, -vertical * rotationSpeed * Time.deltaTime);
            }
        }

        if (Input.GetKey(KeyCode.UpArrow))
            targetFov -= zoomSpeed;
        if (Input.GetKey(KeyCode.DownArrow))
            targetFov += zoomSpeed;

        targetFov = Mathf.Clamp(targetFov, minFov, maxFov);
        cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetFov, ref currentFovVelocity, zoomSmoothTime);

    }

    void RotateCamera(Vector3 direction)
    {
        transform.RotateAround(target.position, direction, rotationSpeed * Time.deltaTime);
    }
    public void setTarget(Transform newTarget)
    {
        target = newTarget;
        StartCoroutine(SmoothMoveAndRotate());
    }

    private IEnumerator SmoothMoveAndRotate()
    {
        isMoving = true; // Start moving

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + distance, target.position.z);
        Quaternion targetRotation = Quaternion.Euler(90, 0, 0);

        // Smoothly move to the target position
        while (Vector3.Distance(transform.position, targetPosition) > 1)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }
        // Ensure we set the final position
        transform.position = targetPosition;

        // Smoothly rotate to the target rotation
        while (Quaternion.Angle(transform.rotation, targetRotation) > 10)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        // Ensure we set the final rotation
        transform.rotation = targetRotation;

        isMoving = false; // Stop moving once we reach the target
    }
}
