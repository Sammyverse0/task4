using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject firstPersonCamera;
    public GameObject thirdPersonCamera;

    private bool isFirstPerson = true;

    void Start()
    {
        SetCameraState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))  // Press V to toggle cameras
        {
            isFirstPerson = !isFirstPerson;
            SetCameraState();
        }
    }

    void SetCameraState()
    {
        firstPersonCamera.SetActive(isFirstPerson);
        thirdPersonCamera.SetActive(!isFirstPerson);

        // Optionally lock/unlock cursor based on camera mode
        
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        
        
        
            
        
    }
}
