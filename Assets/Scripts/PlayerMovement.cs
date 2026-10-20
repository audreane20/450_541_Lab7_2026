using UnityEngine;

// Moves the player with a Rigidbody:
// A/D or Left/Right to move sideways,
// W/S or Up/Down to move forward/backward,
// Space to jump.
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float turnSpeed = 150f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float doubleJumpForce = 4f;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundDistance = 0.5f;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody rb;
    private Vector3 moveDirection;

    private bool isGrounded;
    private bool jumpRequested = false;
    private bool doubleJumpRequested = false;
    private bool hasDoubleJumped = false;

    // Other scripts can check if the player is grounded,
    // but they cannot change this value.
    public bool IsGrounded => isGrounded;

    // LAB 7:
    // Multiplies the normal speed.
    // 1 = normal speed
    // 1.5 = 50% faster
    // Other scripts such as PlayerPowerUps can change this.
    public float SpeedMultiplier { get; set; } = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Prevent the Rigidbody from tipping over.
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        // -------------------------
        // 1. GROUND CHECK
        // -------------------------

        isGrounded = Physics.Raycast(
            transform.position + transform.up * groundDistance / 2f,
            -transform.up,
            groundDistance,
            groundMask
        );

        // Once the player lands, allow another double jump.
        if (isGrounded)
        {
            hasDoubleJumped = false;
        }

        // -------------------------
        // 2. MOVEMENT INPUT
        // -------------------------

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Get the camera directions.
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // We only want movement along the ground.
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Move relative to the camera.
        moveDirection =
            cameraForward * moveZ +
            cameraRight * moveX;

        // Prevent diagonal movement from being faster.
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Rotate the character toward the direction
        // that they are moving.
        if (moveDirection.magnitude > 0f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation =
                Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    turnSpeed * Time.deltaTime
                );
        }

        // -------------------------
        // 3. JUMP INPUT
        // -------------------------

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        // Physics movement should happen in FixedUpdate.
        MovePlayer();

        // First jump.
        if (jumpRequested)
        {
            Jump(jumpForce);
            jumpRequested = false;
        }

        // Double jump.
        if (doubleJumpRequested)
        {
            Jump(doubleJumpForce);
            doubleJumpRequested = false;
        }
    }

    private void MovePlayer()
    {
        // LAB 7:
        // SpeedMultiplier is normally 1.
        // During a speed boost it will become 1.5.
        Vector3 targetVelocity =
            moveDirection * moveSpeed * SpeedMultiplier;

        // Keep the Y velocity so gravity and jumping still work.
        rb.linearVelocity = new Vector3(
            targetVelocity.x,
            rb.linearVelocity.y,
            targetVelocity.z
        );
    }

    private void Jump(float force)
    {
        // Reset vertical speed before jumping.
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        rb.AddForce(
            Vector3.up * force,
            ForceMode.VelocityChange
        );
    }

    // Tries to perform the second jump while in the air.
    // Returns true if the double jump was successfully requested.
    public bool TryDoubleJump()
    {
        // Cannot double jump while grounded.
        // Cannot double jump more than once.
        if (isGrounded || hasDoubleJumped)
        {
            return false;
        }

        doubleJumpRequested = true;
        hasDoubleJumped = true;

        return true;
    }
}