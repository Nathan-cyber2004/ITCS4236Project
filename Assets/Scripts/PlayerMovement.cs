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
    public bool isSprinting { get; private set; }
    [SerializeField] private StaminaBar staminaBar;
    private bool sprintInputMustBeReleased;
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
        sprintInputMustBeReleased = false;
        playerSpeed = walkingSpeed;

        if (staminaBar == null)
        {
            Debug.LogError("Assign a StaminaBar to PlayerMovement in the Inspector.", this);
        }
        else
        {
            staminaBar.setMaxStamina(sprintDuration);
            staminaBar.setStamina(sprintTimer);
        }
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
        bool sprintKeyHeld = Input.GetKey(sprintKey);
        if (!sprintKeyHeld)
        {
            sprintInputMustBeReleased = false;
        }

        bool wantsToSprint = sprintKeyHeld && isPlayerGrounded && isMoving && !sprintInputMustBeReleased;
        isSprinting = wantsToSprint && sprintTimer > 0f;

        if (isSprinting)
        {
            sprintTimer = Mathf.Max(0f, sprintTimer - Time.deltaTime);

            if (sprintTimer <= 0f)
            {
                isSprinting = false;
                sprintInputMustBeReleased = true;
            }
        }
        else
        {
            float rechargeDuration = Mathf.Max(sprintCooldown, 0.01f);
            float rechargeRate = sprintDuration / rechargeDuration;
            sprintTimer = Mathf.Min(sprintDuration, sprintTimer + rechargeRate * Time.deltaTime);
        }

        readyToSprint = sprintTimer > 0f && !sprintInputMustBeReleased;
        playerSpeed = isSprinting ? sprintSpeed : walkingSpeed;
        if (staminaBar != null)
        {
            staminaBar.setStamina(sprintTimer);
        }
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
