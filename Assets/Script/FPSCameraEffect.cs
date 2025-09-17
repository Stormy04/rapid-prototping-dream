using UnityEngine;

public class FirstPersonCameraEffects : MonoBehaviour
{
    public FirstPersonController player; // Assign in Inspector
    private Vector3 defaultLocalPos;
    private float bobTimer;
    private float jumpBobOffset;
    private bool wasJumpingLastFrame;

    public float walkBobSpeed = 8f;
    public float walkBobAmount = 0.05f;
    public float sprintBobSpeed = 14f;
    public float sprintBobAmount = 0.09f;
    public float jumpBobAmount = 0.15f;
    public float jumpBobSpeed = 6f;

    void Start()
    {
        defaultLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (player == null) return;

        bool isMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f;
        bool isGrounded = player.GetComponent<CharacterController>().isGrounded;
        bool isSprinting = player.IsSprinting();
        float verticalVelocity = player.GetVelocity().y;

        // Bobbing
        float bobSpeed = isSprinting ? sprintBobSpeed : walkBobSpeed;
        float bobAmount = isSprinting ? sprintBobAmount : walkBobAmount;
        float bobOffset = 0f;

        if (isMoving && isGrounded)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            bobOffset = Mathf.Sin(bobTimer) * bobAmount;
        }
        else
        {
            bobTimer = 0f;
            bobOffset = 0f;
        }

        // Jump/Land bob (improved)
        if (!isGrounded)
        {
            jumpBobOffset = Mathf.Lerp(jumpBobOffset, -jumpBobAmount, Time.deltaTime * jumpBobSpeed);
            wasJumpingLastFrame = true;
        }
        else
        {
            if (wasJumpingLastFrame)
            {
                jumpBobOffset += jumpBobAmount;
                wasJumpingLastFrame = false;
            }
            jumpBobOffset = Mathf.Lerp(jumpBobOffset, 0f, Time.deltaTime * jumpBobSpeed);
        }

        // Only apply bobbing when grounded, otherwise only jumpBobOffset
        Vector3 targetPos = defaultLocalPos + new Vector3(0, isGrounded ? bobOffset : 0, 0) + new Vector3(0, jumpBobOffset, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 12f);
    }
}
