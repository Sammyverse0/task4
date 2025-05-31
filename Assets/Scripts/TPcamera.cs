using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    

    [Header("References")]
    public Transform target; // The player character
    public Transform cameraHolder; // Empty GameObject that holds the camera
    
    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public float distance = 25f;
    public float minY = -35f;
    public float maxY = 60f;
    
    [Header("Camera Smoothing")]
    public float rotationSmoothTime = 0.12f;
    public float cameraLag = 0.1f;
    
    private float mouseX;
    private float mouseY;
    private Vector3 rotationSmoothVelocity;
    private Vector3 currentRotation;

    void Start()
    {
        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;

        // Set initial rotation
        currentRotation = transform.eulerAngles;

        // If no camera holder is assigned, create one
    }
    
    void Update()
    {
        // Get mouse input
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        // Clamp vertical rotation
        mouseY = Mathf.Clamp(mouseY, minY, maxY);
        
        // Calculate target rotation
        Vector3 targetRotation = new Vector3(mouseY, mouseX);
        
        // Smooth the rotation
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
        
        // Apply rotation
        transform.eulerAngles = currentRotation;
        
        // Follow the target with some lag
        Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, cameraLag);
        transform.position = targetPosition;
    }
    
    void LateUpdate()
{
    
}


}