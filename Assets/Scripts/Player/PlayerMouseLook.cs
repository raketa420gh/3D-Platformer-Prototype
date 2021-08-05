using UnityEngine;

public class PlayerMouseLook : MonoBehaviour
{
    [SerializeField] [Range(0, 100)] private float mouseSensitivity = 50;
    [SerializeField] private Transform body;
    
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseXInput = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseXInput;
        
        body.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
