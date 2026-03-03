using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public GameObject test;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool isGrounded;

    [Header("Dash")]
    // public float dashForce;
    public float dashSpeed;
    //Vector3 dash;
    public bool dashing;
    public float dashDuration;
    public float dashCooldown;
    private float dashCooldownTimer;
    private Vector3 playerFacing;


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

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

        //Animations
        if (horizontalInput != 0 || verticalInput != 0)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        //Dash Ability
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dash();
        }

        //if (dashing)
        //{
        //    moveSpeed = dashSpeed;
        //}
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        Debug.Log(horizontalInput + "<H>V" + verticalInput);
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
        //Vector3 dash = orientation.forward * dashForce;
        //rb.AddForce(dash, ForceMode.Impulse);

        //if (horizontalInput == 0 || verticalInput == 0)
        //{
        //    //playerFacing = orientation.forward;
        //    rb.AddForce(orientation.forward * moveSpeed * 10, ForceMode.Impulse);
        //}
        Debug.Log("DASH");

        Invoke(nameof(ResetDash), dashDuration);

    }

    private void ResetDash()
    {
        animator.SetBool("isDashing", false);
        dashing = false;
        moveSpeed = 4f; // Reset to normal speed
    }
}
