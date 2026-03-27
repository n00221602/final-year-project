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

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

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
        //Grounded check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

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
            //Debug.Log("DASH COOLDOWN: " + dashCooldownTimer);
            if (dashCooldownTimer <= 0)
            {
                dashCooldownTimer = 0;
                //Debug.Log("DASH READY");
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

        if (horizontalInput == 0 && verticalInput == 0)
        {
            rb.linearVelocity = new Vector3(0f, 0, 0f);
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        //Debug.Log(horizontalInput + "<H>V" + verticalInput);
    }

    private void MovePlayer()
    {
        if (combatSystem != null && combatSystem.isAttacking) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //transform.rotation = Quaternion.LookRotation(moveDirection);
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
        //Debug.Log("DASH");

        //ResetDash is called after dashDuration is over.
        Invoke(nameof(ResetDash), dashDuration);

    }

    private void ResetDash()
    {
        animator.SetBool("isDashing", false);
        dashing = false;
        moveSpeed = 4f;

        dashCooldownTimer = 0.8f;
    }

    //private void ResetDashCooldown()
    //{
    //    dashCooldownTimer = 3f;
    //    dashCooldownTimer -= Time.deltaTime;
    //    Debug.Log("DASH COOLDOWN: " + dashCooldownTimer);
    //    if (dashCooldownTimer <= 0)
    //    {
    //        dashCooldownTimer = 0;
    //        Debug.Log("DASH READY");
    //    }
    //}
}
