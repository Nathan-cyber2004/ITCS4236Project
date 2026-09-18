using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public Transform orientation;
    public float playerSpeed;
    public float walkingSpeed = 7f;
    public float sprintSpeed = 12f;
    public float sprintDuration = 5f;
    public float sprintCooldown = 2f;
    public bool readyToSprint;
    public float sprintTimer;
    public float sprintCooldownTimer;
    public bool isSprinting { get; private set; }
    float horizontalInput;
    float verticalInput;
    public float jumpForce = 7f;
    public float jumpCoolDown = 0.25f;
    public float airMultiplier = 0.4f;
    public bool readyToJump;
    Vector3 movementDirection;
    Rigidbody rb;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround = ~0;
    public bool isPlayerGrounded;
    public float groundDrag = 5f;

    [Header("Controls")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        readyToSprint = true;
        sprintTimer = sprintDuration;
        sprintCooldownTimer = 0f;
        playerSpeed = walkingSpeed;
    }

    void Update()
    {
        isPlayerGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        PlayerInput();
        SpeedControl();
    }

    void FixedUpdate()
    {
        MovePlayer();

        if (isPlayerGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && readyToJump && isPlayerGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }

        UpdateSprint();
    }

    private void UpdateSprint()
    {
        bool isMoving = horizontalInput != 0f || verticalInput != 0f;
        bool wantsToSprint = Input.GetKey(sprintKey) && isPlayerGrounded && isMoving;

        // A depleted sprint must finish its full cooldown before it can be used again.
        if (!readyToSprint)
        {
            sprintCooldownTimer = Mathf.Max(0f, sprintCooldownTimer - Time.deltaTime);

            if (sprintCooldownTimer == 0f)
            {
                readyToSprint = true;
                sprintTimer = sprintDuration;
            }
        }

        isSprinting = readyToSprint && wantsToSprint;

        if (isSprinting)
        {
            sprintTimer = Mathf.Max(0f, sprintTimer - Time.deltaTime);

            if (sprintTimer == 0f)
            {
                isSprinting = false;
                readyToSprint = false;
                sprintCooldownTimer = sprintCooldown;
            }
        }

        playerSpeed = isSprinting ? sprintSpeed : walkingSpeed;
    }

    void MovePlayer()
    {
        movementDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (isPlayerGrounded)
        {
            rb.AddForce(movementDirection.normalized * playerSpeed * 10f, ForceMode.Force);
        }
        else if (!isPlayerGrounded)
        {
            rb.AddForce(movementDirection.normalized * playerSpeed * airMultiplier * 10f, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Limit movement speed
        if (flatVel.magnitude > playerSpeed)
        {
            Vector3 limitedVelocity = flatVel.normalized * playerSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        // Reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

}
