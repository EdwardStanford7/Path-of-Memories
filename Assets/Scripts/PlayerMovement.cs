using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Ability activation booleans.
    [SerializeField] public bool canClimb = false;
    [SerializeField] public bool doubleJumpActive = false;
    [SerializeField] public bool dashActive = false;

    // Movement parameters.
    [SerializeField] private float movementSpeed = 8f;
    [SerializeField] private float jumpSpeed = 12f;
    [SerializeField] private float gravityScale = 5.5f;
    [SerializeField] private float airDecelerationFactor = 1.01f;
    [SerializeField] private float groundDecelerationFactor = 1.5f;
    [SerializeField] private float climbingSpeed = 5;
    [SerializeField] private float clingJumpSpeed = 15f;
    [SerializeField] private float jumpBufferTime = .1f;
    [SerializeField] private int numDashes = 1;
    [SerializeField] private int dashStallTime = 5;
    [SerializeField] private int dashSpeed = 50;

    // Layer and collision check variables.
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheckLeft;
    [SerializeField] Transform wallCheckRight;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isFacingRight = true;

    // Player input checks.
    private bool moveRightInput = false;
    private bool moveLeftInput = false;
    private bool moveUpInput = false;
    private bool moveDownInput = false;
    private bool jumpInput = false;
    private bool dashInput = false;

    private int dashesLeft = 1;
    private int jumpsLeft = 1;

    private int jumping = 0;
    private int dashing = 0;

    private bool climbingLastFrame = false;

    // Animation
    private Animator playerAnimator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if ((TouchingWallRight() && canClimb) || dashing > 0)
        {
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
        if (IsGrounded() || (TouchingWallRight() && canClimb))
        {
            dashesLeft = numDashes;

            if (doubleJumpActive)
            {
                jumpsLeft = 2;
            }
            else
            {
                jumpsLeft = 1;
            }
        }

        GetPlayerInputs();
    }


    private void FixedUpdate()
    {
        if (TouchingWallRight() && canClimb)
        {
            rb.velocity = Vector2.zero;
        }

        if (dashInput && dashActive && dashesLeft > 0 && dashing == 0)
        {
            Dash();
        }

        if (jumpInput && jumpsLeft > 0 && !dashInput)
        {
            Jump();
            playerAnimator.SetTrigger("isJumping");
        }

        if (jumping > 0)
        {
            jumping--;
        }
        else
        {
            jumpInput = false;
        }

        if (dashing > 0)
        {
            dashing--;
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (TouchingWallRight() && canClimb && (moveDownInput || moveUpInput) && !jumpInput && !dashInput)
        {
            Climb();
        }

        if (!dashInput && dashing == 0)
        {
            Move();
        }

        // Make the sprite face the right way.
        if (rb.velocity.x < -0.01f)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        if (rb.velocity.x > 0.01f)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // Update the animator.
        playerAnimator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        // If the player dropped off of a wall without jumping remove their jump.
        if (climbingLastFrame != TouchingWallRight())
        {
            if (doubleJumpActive)
            {
                jumpsLeft = 1;
            }
            else
            {
                jumpsLeft = 0;
            }
        }
        climbingLastFrame = TouchingWallRight();
    }

    private void GetPlayerInputs()
    {
        moveLeftInput = false;
        moveRightInput = false;
        moveUpInput = false;
        moveDownInput = false;

        // Arrow key/WASD inputs.
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            moveLeftInput = true;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveRightInput = true;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            moveUpInput = true;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            moveDownInput = true;
        }

        // Jump input.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInput = true;
        }

        // Dash input.
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashActive)
        {
            dashInput = true;
        }
    }

    private void Move()
    {
        if (moveRightInput && !moveLeftInput)
        {
            rb.velocity = new Vector2(movementSpeed, rb.velocity.y);
        }
        else if (moveLeftInput && !moveRightInput)
        {
            rb.velocity = new Vector2(-movementSpeed, rb.velocity.y);
        }
        else if (moveRightInput && moveLeftInput)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            if (IsGrounded())
            {
                rb.velocity = new Vector2((float)(rb.velocity.x / groundDecelerationFactor), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2((float)(rb.velocity.x / airDecelerationFactor), rb.velocity.y);
            }
        }
    }

    private void Jump()
    {
        if (TouchingWallRight())
        {
            rb.velocity = new Vector2(rb.velocity.x, clingJumpSpeed);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        jumping = (int)(jumpBufferTime * 50);
        jumpInput = false;
        jumpsLeft--;
    }

    private void Climb()
    {
        if (moveUpInput && !moveDownInput)
        {
            rb.velocity = new Vector2(0, climbingSpeed);
        }
        else if (moveDownInput && !moveUpInput)
        {
            rb.velocity = new Vector2(0, -climbingSpeed);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    private void Dash()
    {
        if (isFacingRight)
        {
            if (TouchingWallRight() && !IsGrounded())
            {
                rb.velocity = new Vector2(-dashSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(dashSpeed, 0);
            }
        }
        else
        {
            if (TouchingWallRight() && !IsGrounded())
            {
                rb.velocity = new Vector2(dashSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(-dashSpeed, 0);
            }
        }

        dashing = dashStallTime;
        dashInput = false;
        dashesLeft--;
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.02f, groundLayer);
    }

    public bool TouchingWallRight()
    {
        return Physics2D.OverlapCircle(wallCheckRight.position, 0.2f, groundLayer);
    }
}
