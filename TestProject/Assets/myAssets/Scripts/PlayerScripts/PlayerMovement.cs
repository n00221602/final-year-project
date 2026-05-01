using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool isGrounded;

    [Header("Dash")]
    public float dashSpeed;
    public bool dashing;
    public float dashDuration;
    public float dashCooldown;


    private Vector3 playerFacing;
    public float dashCooldownTimer;
    private bool cachedIsGrounded;
    private float groundCheckTimer;
    private const float GROUND_CHECK_INTERVAL = 0.05f;

    public Transform orientation;

    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;

    [HideInInspector] public Vector3 moveDirection;

    [HideInInspector] public Rigidbody rb;

    public Animator animator;
    public CombatSystem combatSystem;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        combatSystem = GetComponent<CombatSystem>();

    }

    private void Update()
    {
        //Grounded check - cache every 50ms to reduce physics calls
        groundCheckTimer += Time.deltaTime;
        if (groundCheckTimer >= GROUND_CHECK_INTERVAL)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
            cachedIsGrounded = isGrounded;
            groundCheckTimer = 0f;
        }
        else
        {
            isGrounded = cachedIsGrounded;
        }

        MyInput();

        SpeedControl();

        //Handle drag
        if (isGrounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

        //Stop sliding immediately when no input


        //Animations
        if (horizontalInput != 0 || verticalInput != 0)
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isIdle", false);
        }
        else
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isIdle", true);
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0)
            {
                dashCooldownTimer = 0;
            }
        }

        //Dash Ability
        if (Input.GetKeyDown(KeyCode.Space) && !combatSystem.isAttacking && dashCooldownTimer == 0)
        {
            Dash();
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //Snap rotation instantly to face the movement direction

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }


    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Dash()
    {
        dashing = true;
        animator.SetBool("isDashing", true);

        moveSpeed = dashSpeed;

        //ResetDash is called after dashDuration is over.
        Invoke(nameof(ResetDash), dashDuration);
    }

    private void ResetDash()
    {
        animator.SetBool("isDashing", false);
        dashing = false;
        moveSpeed = 4f;

        dashCooldownTimer = 0.4f;
    }
}
