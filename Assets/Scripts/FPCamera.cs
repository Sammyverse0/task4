using UnityEngine;

public class FPCamera : MonoBehaviour
{
    public float senx;
    public float seny;
    public Transform Orientation;
    public float xRotation;
    public float yRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mousex = Input.GetAxisRaw("Mouse X") * Time.deltaTime * senx;
        float mousey = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * seny;

        yRotation += mousex;
        xRotation -= mousey;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //rotate player
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Orientation.rotation = Quaternion.Euler(0, yRotation, 0); 
    }
}
