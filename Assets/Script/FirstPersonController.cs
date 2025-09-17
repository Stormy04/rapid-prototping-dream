using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float mouseSensitivity = 2f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    public float sprintDuration = 3f;   // how long you can sprint
    public float sprintCooldown = 5f;   // cooldown before sprint resets

    private CharacterController controller;
    private Transform cam;
    private float verticalRotation = 0f;
    private Vector3 velocity;
    private float coyoteTime = 0.2f; // seconds
    private float coyoteTimeCounter;
    private float sprintTimer;
    private float cooldownTimer;
    private bool isSprinting;
    private bool canSprint = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleSprint();      // Call sprint before movement
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cam.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- Gravity & Jumping ---
        if (controller.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            if (velocity.y < 0)
                velocity.y = -2f;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Allow jump if within coyote time
        if (coyoteTimeCounter > 0f && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            coyoteTimeCounter = 0f; // Prevent double jump
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime); // <-- This line is essential!
    }

    void HandleSprint()
    {
        if (canSprint && Input.GetKey(KeyCode.LeftShift) && controller.isGrounded)
        {
            isSprinting = true;
            sprintTimer += Time.deltaTime;

            if (sprintTimer >= sprintDuration)
            {
                isSprinting = false;
                canSprint = false;
                cooldownTimer = 0f;
            }
        }
        else
        {
            isSprinting = false;
        }

        if (!canSprint)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= sprintCooldown)
            {
                canSprint = true;
                sprintTimer = 0f;
            }
        }
    }
}
