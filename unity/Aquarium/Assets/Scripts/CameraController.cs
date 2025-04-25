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
        // Rotate the camera based on WASD keys
        if (Input.GetKey(KeyCode.W))
        {
            RotateCamera(Vector3.right); // Rotate up
        }
        if (Input.GetKey(KeyCode.S))
        {
            RotateCamera(Vector3.left); // Rotate down
        }
        if (Input.GetKey(KeyCode.A))
        {
            RotateCamera(Vector3.up); // Rotate left
        }
        if (Input.GetKey(KeyCode.D))
        {
            RotateCamera(Vector3.down); // Rotate right
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            targetFov -= zoomSpeed; // Zoom in
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            targetFov += zoomSpeed; // Zoom out
        }

        targetFov = Mathf.Clamp(targetFov, minFov, maxFov);

        cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetFov, ref currentFovVelocity, zoomSmoothTime);

        transform.LookAt(target);
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

    private IEnumerator SmoothMove()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + 100, target.position.z - 10);

        // Smoothly move to the target position
        while (Vector3.Distance(transform.position, targetPosition) > 1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }
        // Ensure we set the final position
        transform.position = targetPosition;

        // Smoothly rotate to the target rotation

    }
    private IEnumerator SmoothMoveAndRotate()
    {
        isMoving = true; // Start moving

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + 100, target.position.z - 10);
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
