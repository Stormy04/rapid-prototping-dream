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
    private Vector3 currentMoveVelocity;
    private Vector3 moveVelocitySmoothDamp;
    public float acceleration = 14f;
    public float deceleration = 18f;
    [Range(0f, 1f)]
    public float airControl = 0.5f;
    private bool isOnFloor;
    private bool wasGroundedLastFrame;
    // Jump buffering and variable jump height
    private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;
    public float lowJumpMultiplier = 2f;
    public bool IsSprinting() => isSprinting;
    public Vector3 GetVelocity() => velocity;
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
        CheckIfOnFloor();
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = (transform.right * moveX + transform.forward * moveZ).normalized;
        float targetSpeed = (isSprinting ? sprintSpeed : walkSpeed) * inputDir.magnitude;

        // Smooth acceleration/deceleration
        float smoothTime = controller.isGrounded
            ? (inputDir.magnitude > 0 ? 1f / acceleration : 1f / deceleration)
            : (1f / acceleration) / Mathf.Lerp(1f, 1f / airControl, 1f - airControl);

        Vector3 targetHorizontalVelocity = inputDir * targetSpeed;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        horizontalVelocity = Vector3.SmoothDamp(
            horizontalVelocity,
            targetHorizontalVelocity,
            ref moveVelocitySmoothDamp,
            smoothTime
        );

        // --- Jump Buffering ---
        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // --- Coyote Time ---
        if ((controller.isGrounded && isOnFloor))
        {
            coyoteTimeCounter = coyoteTime;
            if (velocity.y < 0)
                velocity.y = -2f;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // --- Jumping ---
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
        }

        // --- Variable Jump Height ---
        velocity.y += gravity * Time.deltaTime;
        if (velocity.y > 0 && !Input.GetButton("Jump"))
            velocity.y += gravity * (lowJumpMultiplier - 1) * Time.deltaTime;

        // Combine horizontal and vertical velocity
        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;

        controller.Move(velocity * Time.deltaTime);

        wasGroundedLastFrame = controller.isGrounded && isOnFloor;
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

    void CheckIfOnFloor()
    {
        isOnFloor = false;
        RaycastHit hit;
        float sphereRadius = controller.radius * 0.95f;
        float rayLength = controller.height / 2f + 0.3f;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        if (Physics.SphereCast(rayOrigin, sphereRadius, Vector3.down, out hit, rayLength))
        {
            if (hit.collider.CompareTag("floor"))
            {
                isOnFloor = true;
            }
        }
    }
}
