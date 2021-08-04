using UnityEngine;

public class PlayerMouseLook : MonoBehaviour
{
    [SerializeField] [Range(0, 100)] private float mouseSensitivity = 100;
    
    private float yRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;

        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
