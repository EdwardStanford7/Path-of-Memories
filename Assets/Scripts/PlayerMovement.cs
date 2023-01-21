using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Ability activation booleans.
    private bool canClimb = true; // Climb is holding down left control
    private bool doubleJumpActive = true;
    private bool canDash = true; // dash is left shift

    // Regular movement variables.
    private Rigidbody2D rb;

    public float gravityScale = 10f;

    private bool isFacingRight = true;

    private bool movingLeft = true;
    private bool movingRight = true;

    // Climb movement variables.
    private bool climbInputPressed = false;
    private bool movingUp = false;
    private bool movingDown = false;

    // Jump movement variables.
    private bool grounded = false;

    private float coyoteTime = .15f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = .1f;
    private float jumpBufferCounter;
    private bool jumping = false;

    private bool canDoubleJump = false;
    private bool doubleJumping = false;

    // Dash movement variables.
    private int dashTime = 0;
    private bool dashInput = false;
    private int dashSpeed = 30;
    private bool dashInputReleased = true;

    // Layer and collision check variables.
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheckLeft;
    [SerializeField] Transform wallCheckRight;
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        movingLeft = false;
        movingRight = false;
        dashInput = false;
        jumping = false;

        grounded = IsGrounded();

        if (grounded && doubleJumpActive)
        {
            canDoubleJump = true;
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            movingLeft = true;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            movingRight = true;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            dashInput = true;
        }

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            dashInputReleased = true;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
        {
            jumping = true;

            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !IsGrounded() && canDoubleJump)
        {
            doubleJumping = true;
            canDoubleJump = false;
        }

        if (Input.GetKey(KeyCode.LeftControl) && canClimb && TouchingWall())
        {
            climbInputPressed = true;
            rb.gravityScale = 0;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            climbInputPressed = false;
            movingUp = false;
            movingDown = false;
            rb.gravityScale = gravityScale;
            rb.velocity = Vector2.zero;
        }

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && climbInputPressed)
        {
            movingUp = true;
        }

        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && climbInputPressed)
        {
            movingDown = true;
        }

        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            movingUp = false;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            movingDown = false;
        }
    }

    private void FixedUpdate()
    {
        if ((movingLeft || movingRight) && dashTime == 0)
        {
            if (!movingRight)
            {
                rb.velocity = new Vector2(-10, rb.velocity.y);
            }
            else if (!movingLeft)
            {
                rb.velocity = new Vector2(10, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        else if (dashTime == 0)
        {
            if (!grounded)
            {
                rb.velocity = new Vector2((float)(rb.velocity.x / 1.01), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2((float)(rb.velocity.x / 1.2), rb.velocity.y);
            }
        }

        if (climbInputPressed)
        {
            if (movingUp)
            {
                rb.velocity = new Vector2(0, 5f);
            }
            else if (movingDown)
            {
                rb.velocity = new Vector2(0, -5f);
            }
            else
            {
                rb.velocity = new Vector2(0, 0);
            }
        }

        if (dashInput && canDash && dashInputReleased)
        {
            canDash = false;
            dashInput = false;
            TriggerDash();
            dashInputReleased = false;
        }

        if (dashTime > 5)
        {
            dashTime--;
            rb.velocity = new Vector2(rb.velocity.x, 0);

            return;
        }
        else if (dashTime > 0)
        {
            dashTime--;
            rb.velocity = new Vector2(rb.velocity.x / dashSpeed, 0);

            return;
        }

        if (dashTime == 0)
        {
            if (grounded)
            {
                canDash = true;
            }

            if (!climbInputPressed)
            {
                rb.gravityScale = gravityScale;
            }
        }

        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 15f);

            jumpBufferCounter = 0;
        }

        if (jumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (doubleJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, 25f);
            doubleJumping = false;
        }

        Flip();
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.02f, groundLayer);
    }

    public bool TouchingWall()
    {
        return (Physics2D.OverlapCircle(wallCheckLeft.position, 0.02f, groundLayer) || Physics2D.OverlapCircle(wallCheckRight.position, 0.02f, groundLayer));
    }

    private void Flip()
    {
        if ((isFacingRight && rb.velocity.x < -.1f) || (!isFacingRight && rb.velocity.x > .1f) || (isFacingRight && movingLeft && !movingRight) || (!isFacingRight && movingRight && !movingLeft))
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void TriggerDash()
    {
        rb.gravityScale = 0f;

        if (isFacingRight)
        {
            rb.velocity = new Vector2(dashSpeed, 0);
        }
        else
        {
            rb.velocity = new Vector2(-dashSpeed, 0);
        }

        dashTime = 10;
    }
}
