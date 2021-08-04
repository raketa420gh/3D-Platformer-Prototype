using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerMouseLook))]

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("Jump Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float jumpHeight = 5f;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    
    private Transform selfTransform;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float gravity = -9.81f;

    #endregion
    

    #region Unity lifecycle

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        selfTransform = transform;
    }
    
    private void Update()
    {
        float xInput = Input.GetAxis(AxisNames.Horizontal);
        float zInput = Input.GetAxis(AxisNames.Vertical);

        Vector3 inputDirection = selfTransform.right * xInput + selfTransform.forward * zInput;
        
        Move(inputDirection);
        CheckGround();
        UseGravity();
        
        if (Input.GetButtonDown(AxisNames.Jump) && isGrounded)
        {
            Jump();
        }
    }

    #endregion
    

    #region Private methods

    private void UseGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void Move(Vector3 direction)
    {
        controller.Move(direction * (speed * Time.deltaTime));
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    #endregion
}
