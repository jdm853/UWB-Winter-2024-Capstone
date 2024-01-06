using UnityEngine;
using UnityEngine.InputSystem;

/* This class is based on the implementation from https://www.youtube.com/watch?v=4HpC--2iowE&t=455s&ab_channel=Brackeys 
 and https://www.youtube.com/watch?v=UUJMGQTT5ts&t=91s&ab_channel=iHeartGameDev */

public class ThirdPersonMovementController : MonoBehaviour
{
    // direction & speed
    [SerializeField] private float maxMovementSpeed = 3f;
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float runSpeed = 6.0f;
    private bool isMovementPressed;
    private bool isRunPressed;
    private PlayerInput playerInput;
    private Vector2 horizontalMovementInput;
    private Vector3 movement;
    private Vector3 velocity;

    // jumping
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f * 3;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumpPressed;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;

    // turning
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    [SerializeField] private new Transform camera;

    private CharacterController characterController;

    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        // Input callback listeners
        playerInput.CharacterControls.Move.started += OnMovementInput;  // button fully pressed
        playerInput.CharacterControls.Move.canceled += OnMovementInput; // button fully released
        playerInput.CharacterControls.Move.performed += OnMovementInput;    // button between fully pressed and released (controller)
        
        playerInput.CharacterControls.Run.started += OnRun;
        playerInput.CharacterControls.Run.canceled += OnRun;
        
        playerInput.CharacterControls.Jump.started += OnJump;
        playerInput.CharacterControls.Jump.canceled += OnJump;
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        horizontalMovementInput = context.ReadValue<Vector2>();
        movement.x = horizontalMovementInput.x;
        movement.z = horizontalMovementInput.y; // Vector2 stores values as x,y (not to be confused with unity coordinates x,y,z)
        isMovementPressed = horizontalMovementInput.x != 0 || horizontalMovementInput.y != 0;
    }

    void OnRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();

        if (isGrounded) // only jump if starting from the ground (no jumping mid-air)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            // force player to the ground
            velocity.y = -2f;
        }

        // apply gravity to y axis
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // only move the player if there is input
        if (isMovementPressed)
        {
            // point the player in the direction of input (including current camera rotation)
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + camera.eulerAngles.y;  // get the desired angle including camera rotation
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);  // smooth player turning
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f); // set the player rotation

            // move the player in the direction of input
            float moveSpeed = (isRunPressed ? runSpeed : walkSpeed);
            Vector3 movementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(movementDirection.normalized * moveSpeed * Time.deltaTime);
        }
    }

    public float GetMaxMovementSpeed()
    {
        return maxMovementSpeed;
    }
}
